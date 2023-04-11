using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Megumin.GameFramework.AI.BehaviorTree.Editor
{
    public partial class BehaviorTreeView
    {
        /// <summary>
        /// 正常是将复制的元素序列化为字符串。粘贴时将字符串反序列为新对象
        /// 使用一个取巧的方法做复制粘贴功能。
        /// 使用一个静态成员将要复制的元素保存起来。静态是因为可能再多个编辑器实例中复制粘贴。
        /// 缺点：无法在不同的项目之间实现复制粘贴。
        /// </summary>
        static HashSet<GraphElement> copyedElement = new();

        private string OnSerializeGraphElements(IEnumerable<GraphElement> elements)
        {
            //this.LogMethodName();
            var hashset = elements.ToHashSet();
            if (hashset.Count > 0)
            {
                copyedElement = hashset;
                return "trickCopy";
            }
            return default;
        }

        private bool OnCanPasteSerializedData(string data)
        {
            //this.LogMethodName();
            return data == "trickCopy";
        }

        private void OnUnserializeAndPaste(string operationName, string data)
        {
            //this.LogMethodName(operationName);
            if (data == "trickCopy")
            {
                var upnode = (from elem in copyedElement
                              where elem is Node
                              orderby elem.layout.y
                              select elem).FirstOrDefault();

                var rootPos = Vector2.zero;
                if (upnode != null)
                {
                    rootPos = upnode.layout.position;
                    //可能一次性创建多个节点，这里只注册一次Undo
                    var nodeCount = copyedElement.Count(elem => elem is BehaviorTreeNodeView);
                    using var mute = UndoBeginScope($"Paste {nodeCount} node");

                    Dictionary<BehaviorTreeNodeView, BehaviorTreeNodeView> newPaste = new();
                    foreach (var item in copyedElement)
                    {
                        if (item is BehaviorTreeNodeView nodeView)
                        {
                            var newView = PasteNodeAndView(nodeView, nodeView.layout.position - rootPos);
                            newPaste[nodeView] = newView;
                        }
                    }

                    //复制父子关系。
                    foreach (var item in copyedElement)
                    {
                        if (item is BehaviorTreeNodeView nodeView
                            && newPaste.TryGetValue(nodeView, out var newChildView))
                        {
                            //通过被复制的节点，拿到通过粘贴生成的新阶段
                            foreach (var edge in nodeView.InputPort.connections)
                            {
                                //遍历被复制的节点 的 父节点
                                var parentView = edge.output.node as BehaviorTreeNodeView;
                                if (newPaste.TryGetValue(parentView, out var newParent))
                                {
                                    //如果父节点也被复制，那么连接到被复制的节点
                                    parentView = newParent;
                                }

                                ConnectChild(parentView, newChildView);
                                newChildView.ConnectParentNodeView(parentView);
                            }
                        }
                    }

                    //将选中对象切换到新复制对象
                    ClearSelection();
                    foreach (var item in newPaste)
                    {
                        this.AddToSelection(item.Value);

                        if (item.Key.SONode == Selection.activeObject)
                        {
                            Selection.activeObject = item.Value.SONode;
                            break;
                        }
                    }
                }
            }
        }

        public void ChangeSelection(List<BTNode> nodes)
        {
            ClearSelection();
            foreach (var node in nodes)
            {
                var graphElement = GetElementByGuid(node.GUID);
                if (graphElement != null)
                {
                    AddToSelection(graphElement);
                }
            }
        }

        public void InlineSubtree(ISubtreeTreeElement subtreeNode)
        {
            if (Tree != null
                && subtreeNode is BTNode hostnode
                && subtreeNode.TreeAsset is IBehaviorTreeAsset treeAsset)
            {

                var subtree = treeAsset.Instantiate(editorInitOption, Tree.RefFinder);

                if (subtree.StartNode == null)
                {
                    Debug.LogError("subtree.StartNode == null");
                    return;
                }

                using var mute = UndoBeginScope($"InlineSubtree");

                //合并参数表
                foreach (var item in subtree.Variable.Table)
                {
                    if (Tree.Variable.TryGetParam(item.RefName, out var _))
                    {
                        //父树已经含有同名参数，跳过
                    }
                    else
                    {
                        //将子树参数表加入父树
                        Tree.Variable.Table.Add(item);
                    }
                }

                //合并节点。
                var offsetx = hostnode.Meta.x - subtree.StartNode.Meta.x;
                var offsety = hostnode.Meta.y - subtree.StartNode.Meta.y;

                List<BTNode> added = new();
                foreach (var item in subtree.AllNodes)
                {
                    if (item.Index >= 0)
                    {
                        item.Meta.x += offsetx;
                        item.Meta.y += offsety;
                        //一定要更换GUID，防止冲突
                        item.GUID = Guid.NewGuid().ToString();
                        Tree.AddNode(item);
                        added.Add(item);
                    }
                }

                //直接使用Tree 获取子树父节点，有时候Undo/Redo 后节点找不到树。
                if (Tree.TryGetFirstParent(hostnode, out var parent))
                {
                    Tree.Disconnect(parent, hostnode);
                    Tree.Connect(parent, subtree.StartNode);
                }

                Tree.RemoveNode(hostnode);

                //移动装饰器
                foreach (var item in hostnode.Decorators)
                {
                    subtree.StartNode.AddDecorator(item);
                }

                Tree.UpdateNodeIndexDepth();
                IncrementChangeVersion("InlineSubtree");
                ReloadView();
                Debug.Log("Note: Inline node GUID was changed!");
                ChangeSelection(added);
            }
        }

        public void ConvertToSubtree(BTNode exportNode)
        {
            if (EditorWindow.TryCreateTreeAssset<BehaviorTreeAsset_1_1>(out var asset))
            {
                using var mute = UndoBeginScope($"ConvertToSubtree");
                SubTree subTreeNode = new();
                subTreeNode.GUID = Guid.NewGuid().ToString();
                subTreeNode.BehaviorTreeAsset = asset;
                subTreeNode.Meta = new();
                subTreeNode.Meta.x = exportNode.Meta.x;
                subTreeNode.Meta.y = exportNode.Meta.y;

                BehaviorTree exportTree = new();

                if (Guid.TryParse(asset.GUID, out var _))
                {
                    //尽量使用源资源的树guid
                    exportTree.GUID = asset.GUID;
                }
                else
                {
                    exportTree.GUID = Guid.NewGuid().ToString();
                }

                Tree.AddNode(subTreeNode);
                if (Tree.TryGetFirstParent(exportNode, out var parent))
                {
                    Tree.Disconnect(parent, exportNode);
                    Tree.Connect(parent, subTreeNode);
                }

                void ChangeTree(BTNode node)
                {
                    Tree.RemoveNode(node);
                    exportTree.AddNode(node);
                    //所有节点都要更换GUID，防止父树进行Undo操作，会造成父数和子树的节点ID冲突。
                    node.GUID = Guid.NewGuid().ToString();

                    if (node is BTParentNode parentNode)
                    {
                        foreach (var item in parentNode.Children)
                        {
                            ChangeTree(item);
                        }
                    }
                }
                ChangeTree(exportNode);
                exportTree.StartNode = exportNode;
                exportTree.UpdateNodeIndexDepth();

                asset.SaveTree(exportTree);
                EditorUtility.SetDirty(asset);
                AssetDatabase.SaveAssetIfDirty(asset);
                AssetDatabase.Refresh();

                //引用的参数表无法导出到子树，无法计算引用关系。
                Debug.Log("Note: Export node GUID was changed!  Note: tree variables can not export.");

                Tree.UpdateNodeIndexDepth();
                IncrementChangeVersion("ConvertToSubtree");
                ReloadView();
            }
        }
    }
}




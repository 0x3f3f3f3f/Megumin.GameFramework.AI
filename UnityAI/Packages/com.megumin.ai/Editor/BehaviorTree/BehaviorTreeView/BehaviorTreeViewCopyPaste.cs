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
    }
}

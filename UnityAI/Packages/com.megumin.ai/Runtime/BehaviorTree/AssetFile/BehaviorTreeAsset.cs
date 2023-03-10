using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Codice.Client.BaseCommands.Fileinfo;
using Codice.CM.WorkspaceServer.Tree.GameUI.Checkin.Updater;
using Megumin.GameFramework.AI.Serialization;
using UnityEngine;

namespace Megumin.GameFramework.AI.BehaviorTree
{
    public class BehaviorTreeAsset : ScriptableObject//, ISerializationCallbackReceiver
    {
        public string test = "行为树SO资产";
        public string Comment = "load2";
        public string StartNodeGUID = "";
        public List<NodeAsset> Nodes = new List<NodeAsset>();
        public string Version = AssetVersion.v1_0_0.ToString();

        public static List<string> IgnoreSerializeMember = new()
        {
            nameof(BTNode.Tree),
            nameof(BTNode.Decorators),
            nameof(BTNode.Meta),
            nameof(BTNode.InstanceID),
            nameof(BTNode.GUID),
            nameof(BTParentNode.children),
            nameof(OneChildNode.Child0),
            nameof(TwoChildNode.Child0),
            nameof(TwoChildNode.Child1),
        };

        [Serializable]
        public class NodeAsset : TreeElementAsset
        {
            public string TypeName;
            public string GUID;
            public NodeMeta Meta;
            public List<string> ChildNodes = new();
            public List<DecoratorAsset> Decorators = new();

            //参数使用泛型序列化导致每次保存Rid都会改变
            //[SerializeReference]
            public List<CustomParameterData> MemberData = new();
            public List<CustomParameterData> CallbackMemberData = new();

            public BTNode Instantiate(bool instanceMeta = true)
            {
                var nodeType = Type.GetType(this.TypeName);
                if (nodeType.IsSubclassOf(typeof(BTNode)))
                {
                    var node = Activator.CreateInstance(nodeType) as BTNode;
                    if (node != null)
                    {
                        node.GUID = this.GUID;
                        if (instanceMeta)
                        {
                            node.Meta = this.Meta.Clone();
                        }
                        else
                        {
                            node.Meta = this.Meta;
                        }

                        node.InstanceID = Guid.NewGuid().ToString();

                        //实例化装饰器
                        foreach (var decoratorAsset in Decorators)
                        {
                            var d = decoratorAsset.Instantiate();
                            if (d != null)
                            {
                                node.AddDecorator(d);
                            }
                        }

                        DeserializeMember(node, MemberData, CallbackMemberData);

                        return node;
                    }
                    else
                    {
                        Debug.LogError($"无法创建的节点{this.TypeName}");
                        return null;
                    }
                }
                else
                {
                    Debug.LogError($"无法识别的节点{this.TypeName}");
                    return null;
                }
            }

            public static NodeAsset Serialize(BTNode node, BehaviorTree tree)
            {
                var asset = new NodeAsset();
                asset.TypeName = node.GetType().FullName;
                asset.GUID = node.GUID;
                asset.Meta = node.Meta.Clone();
                asset.Meta.IsStartNode = node == tree.StartNode;

                if (node is BTParentNode parentNode)
                {
                    foreach (var child in parentNode.children)
                    {
                        asset.ChildNodes.Add(child.GUID);
                    }
                }

                //保存装饰器
                if (node.Decorators != null)
                {
                    foreach (var decorator in node?.Decorators)
                    {
                        var decoratorAsset = DecoratorAsset.Serialize(decorator, node, tree);
                        asset.Decorators.Add(decoratorAsset);
                    }
                }

                asset.SerializeMember(node, IgnoreSerializeMember, asset.MemberData, asset.CallbackMemberData);
                return asset;
            }
        }

        [Serializable]
        public class DecoratorAsset : TreeElementAsset
        {
            public string TypeName;
            public string GUID;

            //参数使用泛型序列化导致每次保存Rid都会改变
            //[SerializeReference]
            public List<CustomParameterData> MemberData = new();
            public List<CustomParameterData> CallbackMemberData = new();

            public ITreeElement Instantiate(bool instanceMeta = true)
            {
                var nodeType = Type.GetType(this.TypeName);
                var decorator = Activator.CreateInstance(nodeType) as BTDecorator;

                if (decorator == null)
                {
                    Debug.LogError($"无法创建的装饰器{TypeName}");
                    return decorator;
                }

                decorator.GUID = this.GUID;
                DeserializeMember(decorator, MemberData, CallbackMemberData);

                return decorator;
            }

            public static DecoratorAsset Serialize(object decorator, BTNode node, BehaviorTree tree)
            {
                var asset = new DecoratorAsset();
                asset.TypeName = decorator.GetType().FullName;
                if (decorator is ITreeElement treeElement)
                {
                    asset.GUID = treeElement.GUID;
                }

                asset.SerializeMember(decorator, IgnoreSerializeMember, asset.MemberData, asset.CallbackMemberData);
                return asset;
            }
        }

        public bool SaveTree(BehaviorTree tree)
        {
            if (tree == null)
            {
                return false;
            }

            StartNodeGUID = tree.StartNode?.GUID;

            Nodes.Clear();
            foreach (var node in tree.AllNodes.OrderBy(elem => elem.GUID))
            {
                var nodeAsset = NodeAsset.Serialize(node, tree);
                Nodes.Add(nodeAsset);
            }

            if (Nodes.Count > 0 && !Nodes.Any(elem => elem.Meta.IsStartNode))
            {
                //没有设置开始节点时，将最上面的节点设置为开始节点。
                var upnode = Nodes.OrderBy(elem => elem.Meta.y).FirstOrDefault();
                upnode.Meta.IsStartNode = true;
            }

            //保存参数表


            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="instanceMeta">非调试和编辑状态下，所有树允许共享meta数据，节省性能</param>
        /// <returns></returns>
        public BehaviorTree Instantiate(bool instanceMeta = true)
        {
            var tree = new BehaviorTree();
            tree.InstanceGUID = Guid.NewGuid().ToString();
            foreach (var nodeAsset in Nodes)
            {
                var node = nodeAsset.Instantiate(instanceMeta);
                if (node != null)
                {
                    tree.AddNode(node);
                    if (string.IsNullOrEmpty(StartNodeGUID))
                    {
                        if (nodeAsset.Meta.IsStartNode)
                        {
                            tree.StartNode = node;
                        }
                    }
                    else
                    {
                        if (nodeAsset.GUID == StartNodeGUID)
                        {
                            tree.StartNode = node;
                        }
                    }
                }
            }

            //关联父子关系
            foreach (var nodeAsset in Nodes)
            {
                if (tree.TryGetNodeByGuid<BTParentNode>(nodeAsset.GUID, out var parentNode))
                {
                    foreach (var childNodeAsset in nodeAsset.ChildNodes)
                    {
                        if (tree.TryGetNodeByGuid(childNodeAsset, out var childNode))
                        {
                            parentNode.children.Add(childNode);
                        }
                    }
                }
            }

            tree.Asset = this;
            return tree;
        }

        public void OnBeforeSerialize()
        {
            this.LogMethodName();
        }

        public void OnAfterDeserialize()
        {
            //this.LogFuncName();
        }
    }
}

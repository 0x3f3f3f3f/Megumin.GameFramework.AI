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

        [Serializable]
        public class NodeAsset
        {
            public string TypeName;
            public string GUID;
            public NodeMeta Meta;
            public List<string> ChildNodes = new();
            public List<DecoratorAsset> Decorators = new();
            [SerializeReference]
            public List<ParameterData> ParamAssets = new();

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

                        //反序列化参数
                        foreach (var param in ParamAssets)
                        {
                            param.Instantiate(node);
                        }

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
                var nodeAsset = new NodeAsset();
                nodeAsset.TypeName = node.GetType().FullName;
                nodeAsset.GUID = node.GUID;
                nodeAsset.Meta = node.Meta.Clone();
                nodeAsset.Meta.IsStartNode = node == tree.StartNode;

                if (node is BTParentNode parentNode)
                {
                    foreach (var child in parentNode.children)
                    {
                        nodeAsset.ChildNodes.Add(child.GUID);
                    }
                }

                //保存装饰器
                if (node.Decorators != null)
                {
                    foreach (var decorator in node?.Decorators)
                    {
                        var decoratorAsset = DecoratorAsset.Serialize(decorator, node, tree);
                        nodeAsset.Decorators.Add(decoratorAsset);
                    }
                }

                //保存参数
                //https://github.com/dotnet/runtime/issues/46272
                var nodeType = node.GetType();
                var p = from m in nodeType.GetMembers()
                        where m is FieldInfo || m is PropertyInfo
                        orderby m.MetadataToken
                        select m;
                var members = p.ToList();

                ///用于忽略默认值参数
                var defualtValueNode = Activator.CreateInstance(nodeType);

                foreach (var member in members)
                {
                    if (IgnoreParam.Contains(member.Name))
                    {
                        Debug.LogError($"忽略的参数 {member.Name}");
                        continue;
                    }

                    var paramData = ParameterData.Serialize(member, node, defualtValueNode);
                    if (paramData != null)
                    {
                        nodeAsset.ParamAssets.Add(paramData);
                    }
                }

                return nodeAsset;
            }
        }

        public static List<string> IgnoreParam = new()
        {
            nameof(BTNode.Decorators),
            nameof(BTNode.Meta), 
            nameof(BTNode.InstanceID),
            nameof(BTNode.GUID),
        };
        
        [Serializable]
        public class DecoratorAsset
        {
            public string TypeName;
            public string GUID;

            public ITreeElement Instantiate(bool instanceMeta = true)
            {
                var nodeType = Type.GetType(this.TypeName);
                var decorator = Activator.CreateInstance(nodeType) as BTDecorator;
                decorator.GUID = this.GUID;

                if (decorator == null)
                {
                    Debug.LogError($"无法创建的装饰器{TypeName}");
                }

                return decorator;
            }

            public static DecoratorAsset Serialize(object decorator, BTNode node, BehaviorTree tree)
            {
                var decoratorAsset = new DecoratorAsset();
                decoratorAsset.TypeName = decorator.GetType().FullName;
                if (decorator is ITreeElement treeElement)
                {
                    decoratorAsset.GUID = treeElement.GUID;
                }

                return decoratorAsset;
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

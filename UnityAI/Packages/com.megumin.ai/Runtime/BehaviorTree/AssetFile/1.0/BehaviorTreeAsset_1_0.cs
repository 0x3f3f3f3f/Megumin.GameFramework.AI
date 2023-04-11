using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Megumin.Binding;
using Megumin.Serialization;
using UnityEngine;

namespace Megumin.GameFramework.AI.BehaviorTree
{
    /// <summary>
    /// 序列化架构设计缺陷，不能实现序列化 <![CDATA[List<RefVar<T>>]]> 类型
    /// </summary>
    [Obsolete]
    public partial class BehaviorTreeAsset_1_0 : ScriptableObject
    {
        public string Version = new Version(1, 0, 0).ToString();
        [field: ContextMenuItem("ChangeGUID", "ChangeGUID")]
        [field: SerializeField]
        public string GUID { get; set; } = Guid.NewGuid().ToString();
        public bool UseSerializeReferenceGeneric = false;
        public string test = "行为树SO资产";
        public string Comment = "load2";
        [field: SerializeField]
        public string StartNodeGUID { get; set; } = "";
        public UnityEngine.Object AssetObject => this;

        public List<VariableSerializationData> VariableTable = new();
        public List<NodeAsset> Nodes = new();

        public static List<string> IgnoreSerializeMember = new()
        {
            nameof(BTNode.Tree),
            nameof(BTNode.Decorators),
            nameof(BTNode.Meta),
            nameof(BTNode.InstanceID),
            nameof(BTNode.GUID),
            nameof(BTParentNode.Children),
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
            public List<CollectionSerializationData> MemberData = new();
            public List<CollectionSerializationData> CallbackMemberData = new();

            public BTNode Instantiate(BehaviorTree tree, bool instanceMeta = true)
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
                        node.Tree = tree;

                        //实例化装饰器
                        foreach (var decoratorAsset in Decorators)
                        {
                            var d = decoratorAsset.Instantiate(tree, instanceMeta);
                            if (d != null)
                            {
                                node.AddDecorator(d);
                            }
                        }

                        DeserializeMember(node, MemberData, CallbackMemberData, tree);

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
                    foreach (var child in parentNode.Children)
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
            public List<CollectionSerializationData> MemberData = new();
            public List<CollectionSerializationData> CallbackMemberData = new();

            public IDecorator Instantiate(BehaviorTree tree, bool instanceMeta = true)
            {
                var nodeType = Type.GetType(this.TypeName);
                var decorator = Activator.CreateInstance(nodeType) as BTDecorator;

                if (decorator == null)
                {
                    Debug.LogError($"无法创建的装饰器{TypeName}");
                    return decorator;
                }

                decorator.GUID = this.GUID;
                decorator.Tree = tree;

                DeserializeMember(decorator, MemberData, CallbackMemberData, tree);

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

            if (!Guid.TryParse(tree.GUID, out var _))
            {
                tree.GUID = Guid.NewGuid().ToString();
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
            VariableTable.Clear();
            foreach (var item in tree.Variable.Table)
            {
                VariableSerializationData data = new();
                if (data.TrySerialize(item))
                {
                    VariableTable.Add(data);
                }
            }

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="instanceMeta">非调试和编辑状态下，所有树允许共享meta数据，节省性能</param>
        /// <returns></returns>
        public BehaviorTree Instantiate(InitOption initOption, IRefFinder refFinder = null)
        {
            if (initOption == null)
            {
                return null;
            }

            var instanceMeta = initOption.SharedMeta;
            var tree = new BehaviorTree();
            tree.InstanceGUID = Guid.NewGuid().ToString();

            if (UseSerializeReferenceGeneric)
            {
                //多态序列化 + 泛型
                //
                Debug.Log("至少需要unity2023");
                return tree;
            }


            //反序列化参数表
            foreach (var item in VariableTable)
            {
                if (item.TryDeserialize(out var va))
                {
                    if (va is IRefable variable)
                    {
                        tree.Variable.Table.Add(variable);
                    }
                }
            }


            foreach (var nodeAsset in Nodes)
            {
                var node = nodeAsset.Instantiate(tree, instanceMeta);
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
                            parentNode.Children.Add(childNode);
                        }
                    }
                }
            }

            //tree.Asset = this;
            tree.UpdateNodeIndexDepth();
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

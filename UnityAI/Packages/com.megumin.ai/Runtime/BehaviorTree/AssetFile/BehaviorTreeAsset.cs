using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Codice.CM.WorkspaceServer.Tree.GameUI.Checkin.Updater;
using UnityEngine;

namespace Megumin.GameFramework.AI.BehaviorTree
{
    public class BehaviorTreeAsset : ScriptableObject//, ISerializationCallbackReceiver
    {
        public string test = "行为树SO资产";
        public string Comment = "load2";
        public List<NodeAsset> Nodes = new List<NodeAsset>();

        [Serializable]
        public class NodeAsset
        {
            public string TypeName;
            public string GUID;
            public bool IsStartNode;
            public NodeMeta Meta;
            public List<string> ChildNodes = new List<string>();
        }

        public bool SaveTree(BehaviorTree tree)
        {
            if (tree == null)
            {
                return false;
            }

            Nodes.Clear();
            foreach (var node in tree.AllNodes.OrderBy(elem => elem.GUID))
            {
                var nodeAsset = new NodeAsset();
                nodeAsset.TypeName = node.GetType().FullName;
                nodeAsset.GUID = node.GUID;
                nodeAsset.IsStartNode = node == tree.StartNode;
                nodeAsset.Meta = node.Meta.Clone();

                if (node is BTParentNode parentNode)
                {
                    foreach (var child in parentNode.children)
                    {
                        nodeAsset.ChildNodes.Add(child.GUID);
                    }
                }

                Nodes.Add(nodeAsset);
            }

            if (Nodes.Count > 0 && !Nodes.Any(elem => elem.IsStartNode))
            {
                //没有设置开始节点时，将最上面的节点设置为开始节点。
                var upnode = Nodes.OrderBy(elem => elem.Meta.y).FirstOrDefault();
                upnode.IsStartNode = true;
            }

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="instanceMeta">非调试和编辑状态下，所有树允许共享meta数据，节省性能</param>
        /// <returns></returns>
        public BehaviorTree CreateTree(bool instanceMeta = true)
        {
            var tree = new BehaviorTree();
            foreach (var nodeAsset in Nodes)
            {
                var nodeType = Type.GetType(nodeAsset.TypeName);
                if (nodeType.IsSubclassOf(typeof(BTNode)))
                {
                    var node = Activator.CreateInstance(nodeType) as BTNode;
                    if (node != null)
                    {
                        node.GUID = nodeAsset.GUID;
                        if (instanceMeta)
                        {
                            node.Meta = nodeAsset.Meta.Clone();
                        }
                        else
                        {
                            node.Meta = nodeAsset.Meta;
                        }

                        node.InstanceID = Guid.NewGuid().ToString();
                        tree.AddNode(node);
                        if (nodeAsset.IsStartNode)
                        {
                            tree.StartNode = node;
                        }
                    }
                    else
                    {
                        Debug.LogError($"无法创建的节点{nodeAsset.TypeName}");
                        continue;
                    }
                }
                else
                {
                    Debug.LogError($"无法识别的节点{nodeAsset.TypeName}");
                    continue;
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
            //Load1(tree);
            return tree;
        }

        private void LoadLast(BehaviorTree tree)
        {
            var wait = tree.AddNode<Wait>();
            var log = tree.AddNode<Log>();
            var seq = tree.AddNode<Sequence>();
            seq.children.Add(wait);
            seq.children.Add(log);

            var loop = new Loop();
            seq.Derators = new object[] { loop };
            //var loop = new Repeater();
            //loop.child = seq;

            var check = new CheckBool();
            var remap = new Remap();
            log.Derators = new object[] { check, remap };
            tree.StartNode = seq;
        }

        private void Load4(BehaviorTree tree)
        {
            var wait = new Wait();
            var log = new Log();
            var seq = new Sequence();
            seq.children.Add(wait);
            seq.children.Add(log);

            var loop = new Repeater();
            loop.Child0 = seq;

            var check = new CheckBool();
            var remap = new Remap();
            log.Derators = new object[] { check, remap };
            tree.StartNode = loop;
        }

        private void Load3(BehaviorTree tree)
        {
            var wait = new Wait();
            var log = new Log();
            var seq = new Sequence();
            seq.children.Add(wait);
            seq.children.Add(log);

            var loop = new Repeater();
            loop.Child0 = seq;

            var check = new CheckBool();
            log.Derators = new object[] { check };
            tree.StartNode = loop;
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

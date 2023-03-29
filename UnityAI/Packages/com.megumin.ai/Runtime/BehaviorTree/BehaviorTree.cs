using System;
using System.Collections.Generic;
using System.Linq;
using Megumin.Binding;
using Megumin.Serialization;
using UnityEngine;

namespace Megumin.GameFramework.AI.BehaviorTree
{
    [Serializable]
    public partial class BehaviorTree : AITree
    {
        public string InstanceGUID;
        public TreeMeta TreeMeta;

        [Space]
        public VariableTable Variable = new();

        public readonly Dictionary<string, object> lockDic = new Dictionary<string, object>();
        public BTNode StartNode { get; set; }
        public IBehaviorTreeAsset Asset { get; internal set; }

        [Space]
        [SerializeReference]
        public List<BTNode> AllNodes = new();

        public Dictionary<string, BTNode> GuidDic { get; } = new();
        public bool IsRunning { get; internal set; }

        private Status treestate = Status.Init;



        public void Reset()
        {
            treestate = Status.Init;
        }

        internal void Init(object agent)
        {
            foreach (var item in AllNodes)
            {
                item.Awake();
            }

            ParseAllBindable(agent);

            foreach (var item in AllNodes)
            {
                if (item.Enabled)
                {
                    item.Enable();
                }
            }

            // Start在第一次Tick时调用一次
            //foreach (var item in AllNodes)
            //{
            //    if (item.Enabled && item.IsStarted)
            //    {
            //        item.Start();
            //    }
            //}
        }

        public void ParseAllBindable(object agent, bool force = false)
        {
            Variable.ParseBinding(agent, force);

            foreach (var item in AllElementBindable)
            {
                if (item is IBindingParseable parseable)
                {
                    parseable.ParseBinding(agent, force);
                }
            }
        }

        static readonly Unity.Profiling.ProfilerMarker tickProfilerMarker = new(nameof(Tick));

        /// <summary>
        /// Todo 抽象出runner ，分别 root - leaf 驱动，last leaf， 异步。三种方式根调用不一样。但是都需要Tick。
        /// </summary>
        /// <returns></returns>
        public Status Tick()
        {
            using var profiler = tickProfilerMarker.Auto();

            if (StartNode == null)
            {
                return Status.Failed;
            }

            if (StartNode.Enabled == false)
            {
                Debug.Log($"StartNode is not Enabled!");
                return Status.Failed;
            }

            if (treestate == Status.Succeeded)
            {
                if (RunOption != null && RunOption.OnSucceeded.HasFlag(OperationTree.ReStart))
                {
                    treestate = Status.Init;
                }
                else
                {
                    return Status.Succeeded;
                }
            }

            if (treestate == Status.Failed)
            {
                if (RunOption != null && RunOption.OnFailed.HasFlag(OperationTree.ReStart))
                {
                    treestate = Status.Init;
                }
                else
                {
                    return Status.Failed;
                }
            }

            treestate = StartNode.Tick(null);
            if (treestate == Status.Succeeded || treestate == Status.Failed)
            {
                Debug.Log($"tree complate. {treestate}");
            }

            return treestate;
        }

        public BTNode AddNode(BTNode node)
        {
            node.Tree = this;
            AllNodes.Add(node);
            GuidDic[node.GUID] = node;
            return node;
        }

        public bool RemoveNode(BTNode node)
        {
            if (node == null)
            {
                return false;
            }

            if (node.Tree == this)
            {
                node.Tree = null;
            }

            GuidDic.Remove(node.GUID);
            return AllNodes.Remove(node);
        }


        public T AddNode<T>() where T : BTNode, new()
        {
            var node = new T();
            node.GUID = Guid.NewGuid().ToString();
            node.InstanceID = Guid.NewGuid().ToString();
            AddNode(node);
            return node;
        }

        internal BTNode AddNewNode(Type type)
        {
            if (type.IsSubclassOf(typeof(BTNode)))
            {
                var node = Activator.CreateInstance(type) as BTNode;
                if (node != null)
                {
                    node.GUID = Guid.NewGuid().ToString();
                    AddNode(node);
                }
                return node;
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        public BTNode GetNodeByGuid(string guid)
        {
            if (GuidDic.TryGetValue(guid, out var node))
            {
                return node;
            }
            return default;
        }

        public bool TryGetNodeByGuid(string guid, out BTNode node)
        {
            return GuidDic.TryGetValue(guid, out node);
        }

        public bool TryGetNodeByGuid<T>(string guid, out T node)
            where T : BTNode
        {
            if (GuidDic.TryGetValue(guid, out var tempNode))
            {
                if (tempNode is T castNode)
                {
                    node = castNode;
                    return true;
                }
            }
            node = null;
            return false;
        }

        public bool IsStartNodeByGuid(string guid)
        {
            if (string.IsNullOrEmpty(guid))
            {
                return false;
            }
            return StartNode?.GUID == guid;
        }

        /// <summary>
        /// 是不是开始节点的子代
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        internal bool IsStartNodeDescendant(BTNode node)
        {
            if (node == null)
            {
                return false;
            }

            if (StartNode is BTParentNode parentNode)
            {
                return parentNode.IsDescendant(node);
            }

            return false;
        }

        internal void UpdateNodeIndexDepth()
        {
            foreach (var item in AllNodes)
            {
                if (item.Meta != null)
                {
                    item.Meta.index = -1;
                    item.Meta.depth = -1;
                }
            }

            var index = 0;
            void SetNodeIndex(BTNode node, int depth = 0)
            {
                if (node == null)
                {
                    return;
                }

                if (node.Meta != null)
                {
                    node.Meta.index = index;
                    node.Meta.depth = depth;
                }

                index++;

                if (node is BTParentNode parentNode)
                {
                    var nextDepth = depth + 1;
                    foreach (var child in parentNode.children)
                    {
                        SetNodeIndex(child, nextDepth);
                    }
                }
            }

            SetNodeIndex(StartNode);
        }

        public bool TryGetFirstParent(BTNode node, out BTParentNode parentNode)
        {
            foreach (var item in AllNodes)
            {
                if (item is BTParentNode p)
                {
                    if (p.ContainsChild(node))
                    {
                        parentNode = p;
                        return true;
                    }
                }
            }

            parentNode = null;
            return false;
        }
    }

    public partial class BehaviorTree
    {
        /// <summary>
        /// 用于编辑中UndoRedo时实例对象改变。
        /// </summary>
        public void ReCacheDic()
        {
            GuidDic.Clear();
            foreach (var node in AllNodes)
            {
                GuidDic.Add(node.GUID, node);
                if (node.GUID == Asset?.StartNodeGUID)
                {
                    StartNode = node;
                }
            }
        }
    }

    public partial class BehaviorTree : IRefFinder
    {
        bool IRefFinder.TryGetRefValue(string refName, out object refValue)
        {
            if (Variable.TryGetParam(refName, out var param))
            {
                refValue = param;
                return true;
            }

            refValue = null;
            return false;
        }
    }
}

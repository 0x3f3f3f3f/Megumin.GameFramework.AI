using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Megumin.Binding;
using Megumin.Serialization;
using UnityEngine;

namespace Megumin.AI.BehaviorTree
{
    [Serializable]
    public partial class BehaviorTree : AITree
    {
        public string InstanceGUID;
        public TreeMeta TreeMeta;

        [Space]
        public VariableTable Variable = new();

        public Dictionary<string, object> LockDic { get; } = new();
        public BTNode StartNode { get; set; }
        public IBehaviorTreeAsset Asset { get; internal set; }


        public InitOption InitOption { get; set; }
        public RefFinder RefFinder { get; set; }

        public override void Reset()
        {
            base.Reset();
            treestate = Status.Init;
        }

        public void ReStart()
        {
            if (StartNode != null && StartNode.State == Status.Running)
            {
                StartNode.Abort(StartNode);
            }
            treestate = Status.Init;
        }

        protected static readonly Unity.Profiling.ProfilerMarker ParseAllBindableMarker = new("ParseAllBindable");

        /// <summary>
        /// 内部GetCompment只能在主线程调用
        /// </summary>
        /// <param name="agent"></param>
        /// <param name="force"></param>
        public virtual void ParseAllBindable(object agent, bool force = false, object options = null)
        {
            using var profiler = ParseAllBindableMarker.Auto();

            Variable.ParseBinding(agent, force, options);

            foreach (var item in AllBindingParseable)
            {
                if (item is IBindingParseable parseable)
                {
                    parseable.ParseBinding(agent, force, options);
                }
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

        public bool TryGetFirstExetutePath(BTNode node, List<BTParentNode> exetutePath)
        {
            if (node == null)
            {
                return false;
            }

            if (StartNode is BTParentNode parentNode)
            {
                return parentNode.IsDescendant(node, exetutePath);
            }

            return false;
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

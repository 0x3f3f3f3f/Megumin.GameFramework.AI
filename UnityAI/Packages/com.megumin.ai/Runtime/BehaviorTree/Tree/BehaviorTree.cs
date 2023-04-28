using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
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

        public Dictionary<string, object> LockDic { get; } = new();
        public BTNode StartNode { get; set; }
        public IBehaviorTreeAsset Asset { get; internal set; }


        public InitOption InitOption { get; set; }
        public RefFinder RefFinder { get; set; }

        public void Reset()
        {
            foreach (var item in AllNodes)
            {
                item.Reset();
            }
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

        static readonly Unity.Profiling.ProfilerMarker parseAllBindableMarker = new("ParseAllBindable");

        /// <summary>
        /// 内部GetCompment只能在主线程调用
        /// </summary>
        /// <param name="agent"></param>
        /// <param name="force"></param>
        public void ParseAllBindable(object agent, bool force = false, object options = null)
        {
            using var profiler = parseAllBindableMarker.Auto();
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

    public partial class BehaviorTree
    {
        public bool IsRunning { get; internal set; }

        private Status treestate = Status.Init;

        public string InstanceName { get; set; } = "anonymity";

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

    public partial class BehaviorTree
    {
        //Realtime Data
        public int CompletedCount { get; protected set; } = 0;
        public int SucceededCount { get; protected set; } = 0;
        public int FailedCount { get; protected set; } = 0;
        /// <summary>
        /// 可以用于节点区分是不是当前tick。
        /// </summary>
        public int TotalTickCount { get; protected set; } = 0;
        /// <summary>
        /// 当前正在Tick的节点序号
        /// </summary>
        public int LastTickNodeIndex { get; set; } = -1;
        public BTNode LastTick { get; set; } = null;

        /// <summary>
        /// 事件生命周期为一个tick
        /// </summary>
        Dictionary<string, EventData> eventCache = new();

        public class EventData
        {
            public BTNode SendNode { get; set; }
            public int SendTick { get; set; }
        }

        List<string> removeKey = new();
        protected void RemoveLifeEndEventData()
        {
            if (eventCache.Count > 0)
            {
                removeKey.Clear();
                foreach (var item in eventCache)
                {
                    if (item.Value.SendTick + 1 < TotalTickCount)
                    {
                        //大于1个tick的事件数据被删除
                        removeKey.Add(item.Key);
                    }
                }

                foreach (var item in removeKey)
                {
                    eventCache.Remove(item);
                }
                removeKey.Clear();
            }
        }

        public void SendEvent(string eventName, BTNode sendNode = null)
        {
            if (string.IsNullOrEmpty(eventName))
            {
                return;
            }

            EventData eventData = new();
            eventData.SendTick = TotalTickCount;
            eventData.SendNode = sendNode;
            eventCache[eventName] = eventData;
        }

        public bool TryGetEvent(string eventName, BTNode checkNode, out object eventData)
        {
            if (string.IsNullOrEmpty(eventName))
            {
                eventData = null;
                return false;
            }

            if (eventCache.TryGetValue(eventName, out var evtData))
            {
                eventData = evtData;
                //事件的生命周期是发送节点 到下次一次tick 发送节点，总共一个Tick
                if (evtData.SendNode == null)
                {
                    //当前Tick访问都认为是触发事件
                    return TotalTickCount == evtData.SendTick;
                }
                else
                {
                    if (TotalTickCount > evtData.SendTick)
                    {
                        //下一次Tick 除了 发送节点之前的节点和发送节点本身 认为可以收到事件
                        return IsBehind(evtData.SendNode, checkNode) == false;
                    }
                    else
                    {
                        //当前Tick 发送节点之后执行的节点认为可以收到事件
                        return IsBehind(evtData.SendNode, checkNode);
                    }
                }

            }
            eventData = null;
            return false;
        }

        /// <summary>
        /// 测试节点是不是在给定节点之后
        /// </summary>
        /// <param name="positionNode"></param>
        /// <param name="checkNode"></param>
        /// <returns></returns>
        public bool IsBehind(BTNode positionNode, BTNode checkNode)
        {
            if (positionNode == null)
            {
                return true;
            }

            if (checkNode == null)
            {
                return false;
            }

            if (positionNode.Tree == checkNode.Tree)
            {
                return positionNode.Index < checkNode.Index;
            }
            else
            {
                //Todo 子树。
                throw new NotImplementedException();
            }

            //return false;
        }

        /// <summary>
        /// 事件生命周期为一个tick
        /// </summary>
        Dictionary<string, TriggerData> triggerCache = new();
        public class TriggerData
        {
            public BTNode SendNode { get; set; }
            public int SendTick { get; set; }
        }

        public void SetTrigger(string triggerName, BTNode sendNode = null)
        {
            if (string.IsNullOrEmpty(triggerName))
            {
                return;
            }

            TriggerData eventData = new();
            eventData.SendTick = TotalTickCount;
            eventData.SendNode = sendNode;
            triggerCache[triggerName] = eventData;
        }

        public bool TryGetTrigger(string triggerName, out TriggerData triggerData)
        {
            if (string.IsNullOrEmpty(triggerName))
            {
                triggerData = null;
                return false;
            }

            if (triggerCache.TryGetValue(triggerName, out triggerData))
            {
                return true;
            }

            triggerData = null;
            return false;
        }

        public void ResetTrigger(string triggerName)
        {
            triggerCache.Remove(triggerName);
        }
    }
}

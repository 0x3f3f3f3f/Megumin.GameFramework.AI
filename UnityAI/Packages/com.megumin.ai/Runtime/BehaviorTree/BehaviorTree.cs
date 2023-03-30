using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
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

            RemoveLifeEndEventData();
            treestate = StartNode.Tick(null);

            TotalTickCount++;

            if (treestate == Status.Succeeded)
            {
                CompletedCount++;
                SucceededCount++;
                Debug.Log($"tree complate. {treestate}");
            }
            else if (treestate == Status.Failed)
            {
                CompletedCount++;
                FailedCount++;
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
        /// 事件存在一个tick
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

        public void SendEvent(string eventName, BTNode sendNode)
        {
            EventData eventData = new();
            eventData.SendTick = TotalTickCount;
            eventData.SendNode = sendNode;
            eventCache[eventName] = eventData;
        }

        public bool TryGetEvent(string eventName, BTNode checkNode, out object eventData)
        {
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
                return positionNode.Meta.index < checkNode.Meta.index;
            }
            else
            {
                //Todo 子树。
                throw new NotImplementedException();
            }

            //return false;
        }
    }
}

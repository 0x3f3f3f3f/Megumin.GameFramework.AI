using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Megumin.AI.BehaviorTree
{
    public partial class BehaviorTree
    {
        /// <summary>
        /// 事件生命周期为一个tick，事件可以被多次响应
        /// </summary>
        Dictionary<object, EventData> eventCache = new();

        public class EventData
        {
            public object Evt { get; set; }
            public object Arg1 { get; internal set; }
            public object Arg2 { get; internal set; }
            public object Arg3 { get; internal set; }

            /// <summary>
            /// 由行为树外部触发时，值为null
            /// </summary>
            public BTNode SendNode { get; set; }
            public int SendTick { get; set; }
            public int UsedCount { get; set; }
            public BehaviorTree Tree { get; set; }
            public int Priority { get; internal set; }
        }

        List<object> removeKey = new();
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

        public bool SendEvent<T>(T evt,
                                 BTNode sendNode = null,
                                 object arg1 = null,
                                 object arg2 = null,
                                 object arg3 = null,
                                 int priority = 0)
        {
            if (evt is null)
            {
                return false;
            }

            if (eventCache.TryGetValue(evt, out var oldData))
            {
                if (oldData.Priority > priority)
                {
                    //已经存在的触发器权重大于新触发器的权重，则忽略新触发器
                    return false;
                }
            }

            EventData eventData = new();
            eventData.Tree = this;
            eventData.SendTick = TotalTickCount;
            eventData.SendNode = sendNode;

            eventData.Evt = evt;
            eventData.Arg1 = arg1;
            eventData.Arg2 = arg2;
            eventData.Arg3 = arg3;
            eventData.Priority = priority;
            eventCache[evt] = eventData;

            return true;
        }

        public bool TryGetEvent<T>(T evt, BTNode checkNode, out EventData eventData)
        {
            if (evt is null)
            {
                eventData = null;
                return false;
            }

            if (eventCache.TryGetValue(evt, out var evtData))
            {
                eventData = evtData;
                if (CheckTimeOut(evtData, checkNode))
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            eventData = null;
            return false;
        }

        /// <summary>
        /// 根据事件类型获取，因为用了Dictionary，多个同类型同时存在时，不确定返回哪一个。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool TryGetEvent<T>(BTNode checkNode, out EventData data)
        {
            foreach (var item in eventCache)
            {
                if (item.Key is T)
                {
                    if (CheckTimeOut(item.Value, checkNode))
                    {
                        continue;
                    }

                    data = item.Value;
                    return true;
                }
            }

            data = null;
            return false;
        }

        public void RemoveEvent<T>(T evt)
        {
            if (evt is null)
            {
                return;
            }

            eventCache.Remove(evt);
        }

        /// <summary>
        /// 根据节点位置，判断事件是否超时
        /// </summary>
        /// <param name="evtData"></param>
        /// <param name="checkNode"></param>
        /// <returns></returns>
        public bool CheckTimeOut(EventData evtData, BTNode checkNode)
        {
            //事件的生命周期是发送节点 到下次一次tick 发送节点，总共一个Tick

            if (TotalTickCount <= evtData.SendTick)
            {
                //当前执行Tick，不超时
                return false;
            }
            else
            {
                //发送事件之后一帧的Tick

                if (evtData.SendNode == null)
                {
                    //行为树外部触发的事件，执行Tick大于发送Tick认为是超时。
                    return true;
                }
                else
                {
                    if (IsBehind(evtData.SendNode, checkNode))
                    {
                        //测试节点在发送节点后面，那么在上一个Tick以及执行过了，认为是超时。
                        return true;
                    }
                }
            }

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
    }
}






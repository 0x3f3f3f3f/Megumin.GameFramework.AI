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
        Dictionary<object, EventContext> eventCache = new();

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

            EventData<T> eventData = new();
            eventData.Tree = this;
            eventData.SendTick = TotalTickCount;
            eventData.SendNode = sendNode;

            eventData.Event = evt;
            eventData.Arg1 = arg1;
            eventData.Arg2 = arg2;
            eventData.Arg3 = arg3;
            eventData.Priority = priority;
            eventCache[evt] = eventData;

            return true;
        }

        public bool TryGetEvent<T>(T evt, out EventData<T> eventData, BTNode checkNode)
        {
            if (evt is null)
            {
                eventData = null;
                return false;
            }

            if (eventCache.TryGetValue(evt, out var evtData))
            {
                if (evtData is EventData<T> gData)
                {
                    eventData = gData;
                    if (CheckTimeOut(gData.SendTick, gData.SendNode, checkNode))
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
            }
            eventData = null;
            return false;
        }


        /// <summary>
        /// 根据事件类型获取，因为用了Dictionary，多个同类型同时存在时，不确定返回哪一个。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="eventData"></param>
        /// <returns></returns>
        public bool TryGetEvent<T>(out EventData<T> eventData, BTNode checkNode)
        {
            foreach (var item in eventCache)
            {
                if (item.Key is T && item.Value is EventData<T> gData)
                {
                    if (CheckTimeOut(gData.SendTick, gData.SendNode, checkNode))
                    {
                        continue;
                    }

                    eventData = gData;
                    return true;
                }
            }

            eventData = null;
            return false;
        }

        /// <summary>
        /// 根据类型获取所有事件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="eventDatas"></param>
        /// <param name="checkNode"></param>
        /// <returns></returns>
        public bool TryGetEvents<T>(List<EventData<T>> eventDatas, BTNode checkNode)
        {
            bool hasValue = false;
            foreach (var item in eventCache)
            {
                if (item.Key is T && item.Value is EventData<T> gData)
                {
                    if (CheckTimeOut(gData.SendTick, gData.SendNode, checkNode))
                    {
                        continue;
                    }

                    eventDatas.Add(gData);
                    hasValue = true;
                }
            }

            return hasValue;
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
        /// <param name="eventSendTick"></param>
        /// <param name="eventSendNode"></param>
        /// <param name="checkNode"></param>
        /// <returns></returns>
        public bool CheckTimeOut(int eventSendTick, BTNode eventSendNode, BTNode checkNode)
        {
            //事件的生命周期是发送节点 到下次一次tick 发送节点，总共一个Tick

            if (TotalTickCount <= eventSendTick)
            {
                //当前执行Tick，不超时
                return false;
            }
            else
            {
                //发送事件之后一帧的Tick

                if (eventSendNode == null)
                {
                    //行为树外部触发的事件，执行Tick大于发送Tick认为是超时。
                    return true;
                }
                else
                {
                    if (IsBehind(eventSendNode, checkNode))
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






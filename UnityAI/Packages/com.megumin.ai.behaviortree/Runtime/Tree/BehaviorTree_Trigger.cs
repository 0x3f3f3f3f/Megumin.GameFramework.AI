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
        /// 触发器可以长久存在，直到被Reset，通常时获取触发器后，立刻Reset，也就是设计为仅被响应一次。
        /// </summary>
        internal Dictionary<object, EventContext> triggerCache = new();

        [Obsolete("Use SendTrigger instead.")]
        public bool SetTrigger<T>(T trigger,
                                  BTNode sendNode = null,
                                  object arg1 = null,
                                  object arg2 = null,
                                  object arg3 = null,
                                  int priority = 0) => SendTrigger(trigger, sendNode, arg1, arg2, arg3, priority);

        public bool SendTrigger<T>(T trigger,
                                  BTNode sendNode = null,
                                  object arg1 = null,
                                  object arg2 = null,
                                  object arg3 = null,
                                  int priority = 0)
        {
            if (trigger is null)
            {
                return false;
            }

            if (triggerCache.TryGetValue(trigger, out var oldData))
            {
                if (oldData.Priority > priority)
                {
                    //已经存在的触发器权重大于新触发器的权重，则忽略新触发器
                    return false;
                }
            }

            TriggerData<T> eventData = new();
            eventData.Tree = this;
            eventData.SendTick = TotalTickCount;
            eventData.SendNode = sendNode;

            eventData.Trigger = trigger;
            eventData.Arg1 = arg1;
            eventData.Arg2 = arg2;
            eventData.Arg3 = arg3;
            eventData.Priority = priority;
            triggerCache[trigger] = eventData;

            return true;
        }

        public bool TryGetTrigger<T>(T trigger, out TriggerData<T> triggerData)
        {
            if (trigger is null)
            {
                triggerData = null;
                return false;
            }

            if (triggerCache.TryGetValue(trigger, out var data))
            {
                if (data is TriggerData<T> gData)
                {
                    triggerData = gData;
                    return true;
                }
            }

            triggerData = null;
            return false;
        }

        /// <summary>
        /// 根据事件类型获取，因为用了Dictionary，多个同类型同时存在时，不确定返回哪一个。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool TryGetTrigger<T>(out TriggerData<T> data)
        {
            foreach (var item in triggerCache)
            {
                if (item.Key is T && item.Value is TriggerData<T> gData)
                {
                    data = gData;
                    return true;
                }
            }

            data = null;
            return false;
        }

        /// <summary>
        /// 根据类型获取所有事件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="eventDatas"></param>
        /// <returns></returns>
        public bool TryGetTriggers<T>(List<TriggerData<T>> eventDatas)
        {
            bool hasValue = false;
            foreach (var item in triggerCache)
            {
                if (item.Key is T && item.Value is TriggerData<T> gData)
                {
                    eventDatas.Add(gData);
                    hasValue = true;
                }
            }

            return hasValue;
        }

        [Obsolete("use RemoveTrigger instead.")]
        public void ResetTrigger(object trigger)
        {
            triggerCache.Remove(trigger);
        }

        public void RemoveTrigger<T>(T trigger)
        {
            if (trigger is null)
            {
                return;
            }

            triggerCache.Remove(trigger);
        }
    }
}

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
        Dictionary<object, TriggerData> triggerCache = new();

        public class TriggerData
        {
            public object Trigger { get; set; }
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

            public void Use()
            {
                UsedCount++;
                if (Tree != null)
                {
                    Tree.triggerCache.Remove(Trigger);
                }
            }
        }

        public bool SetTrigger<T>(T trigger,
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

            TriggerData eventData = new();
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

        public bool TryGetTrigger<T>(T trigger, out TriggerData triggerData)
        {
            if (trigger is null)
            {
                triggerData = null;
                return false;
            }

            if (triggerCache.TryGetValue(trigger, out triggerData))
            {
                return true;
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
        public bool TryGetTrigger<T>(out TriggerData data)
        {
            foreach (var item in triggerCache)
            {
                if (item.Key is T)
                {
                    data = item.Value;
                    return true;
                }
            }

            data = null;
            return false;
        }

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

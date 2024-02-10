using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Megumin.AI.BehaviorTree
{
    public class EventContext
    {
        /// <summary>
        /// 事件进入行为树时的TotalTickCount。
        /// </summary>
        public int SendTick { get; set; }
        public int Priority { get; set; }
        public int UsedCount { get; set; }

        public object Arg1 { get; set; }
        public object Arg2 { get; set; }
        public object Arg3 { get; set; }

        /// <summary>
        /// 由行为树外部触发时，值为null
        /// </summary>
        public BTNode SendNode { get; set; }
        public BehaviorTree Tree { get; set; }
    }

    public class EventData<T> : EventContext
    {
        public T Event { get; set; }
    }

    public class TriggerData<T> : EventContext
    {
        public T Trigger { get; set; }
        public void Use()
        {
            UsedCount++;
            if (Tree != null)
            {
                Tree.triggerCache.Remove(Trigger);
            }
        }
    }

}

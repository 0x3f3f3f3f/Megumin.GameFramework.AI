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

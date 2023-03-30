using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Megumin.Binding;

namespace Megumin.GameFramework.AI.BehaviorTree
{
    public class CheckTrigger : ConditionDecorator, IDetailable
    {
        public RefVar<string> TriggerName;

        protected override bool OnCheckCondition(BTNode container)
        {
            if (Tree.TryGetTrigger(TriggerName, out var eventData))
            {
                return true;
            }
            return false;
        }

        public string GetDetail()
        {
            return @$"Trg: ""{TriggerName?.Value}""";
        }
    }
}

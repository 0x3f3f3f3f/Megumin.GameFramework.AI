using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Megumin.Binding;

namespace Megumin.GameFramework.AI.BehaviorTree
{
    public class CheckEvent : ConditionDecorator,ITitleable
    {
        public RefVar<string> EventName;

        protected override bool OnCheckCondition(BTNode container)
        {
            if (Tree.TryGetEvent(EventName, container, out var eventData))
            {
                return true;
            }
            return base.OnCheckCondition(container);
        }

        public string Title => $"CheckEvent {EventName?.Value}";
    }
}


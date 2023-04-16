using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Megumin.Binding;

namespace Megumin.GameFramework.AI.BehaviorTree
{
    public class CheckEvent : ConditionDecorator, IDetailable
    {
        public RefVar_String EventName;

        protected override bool OnCheckCondition(object options = null)
        {
            if (Tree.TryGetEvent(EventName, Owner, out var eventData))
            {
                return true;
            }
            return false;
        }

        public string GetDetail()
        {
            return @$"Name: ""{(string)EventName}""";
        }
    }
}


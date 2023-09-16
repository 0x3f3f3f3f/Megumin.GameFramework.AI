using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Megumin.Binding;
using Megumin.Reflection;
using UnityEngine;

namespace Megumin.AI.BehaviorTree
{
    [DisplayName("CheckTimeout")]
    [SerializationAlias("Megumin.AI.BehaviorTree.CheckTimeout")]
    public class CheckTimeout_Decorator : ConditionDecorator, IPreDecorator
    {
        public CheckTimeout_Decorator()
        {
            AbortType = AbortType.Self;
        }

        public RefVar_Float Duration = new() { value = 30f };
        float enterTime;

        public void BeforeNodeEnter(object options = null)
        {
            enterTime = Time.time;
        }

        protected override bool OnCheckCondition(object options = null)
        {
            if (Owner.State == Status.Running)
            {
                return Time.time - enterTime <= Duration;
            }
            return true;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Megumin.Binding;
using UnityEngine;

namespace Megumin.GameFramework.AI.BehaviorTree
{
    public class CheckTimeout : ConditionDecorator, IPreDecorator
    {
        public CheckTimeout()
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

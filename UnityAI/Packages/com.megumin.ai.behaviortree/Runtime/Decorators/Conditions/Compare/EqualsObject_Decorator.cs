using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Megumin.Binding;
using UnityEngine;

namespace Megumin.AI.BehaviorTree
{
    [DisplayName("EqualsObject")]
    public class EqualsObject_Decorator : ConditionDecorator
    {
        public RefVar_SystemObject Left;
        public RefVar_SystemObject Right;

        protected override bool OnCheckCondition(object options = null)
        {
            return System.Object.Equals(Left, Right);
        }
    }
}

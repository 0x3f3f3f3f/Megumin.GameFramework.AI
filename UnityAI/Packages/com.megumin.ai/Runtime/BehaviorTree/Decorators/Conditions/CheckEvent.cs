﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Megumin.Binding;

namespace Megumin.GameFramework.AI.BehaviorTree
{
    public class CheckEvent : ConditionDecorator
    {
        public RefVar<string> EventName;

        protected override bool OnCheckCondition()
        {
            if (Tree.TryGetEvent(EventName, out var eventData))
            {
                return true;
            }
            return base.OnCheckCondition();
        }
    }
}


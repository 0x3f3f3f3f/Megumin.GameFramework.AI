﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Megumin.Binding;

namespace Megumin.GameFramework.AI.BehaviorTree
{
    public enum WhenResetTrigger
    {
        Immediate,
        EnterNode,
        LeaveNode,
    }

    public class CheckTrigger : ConditionDecorator, IDetailable, IPreDecorator, IPostDecorator, IAbortDecorator
    {
        public RefVar_String TriggerName;
        public WhenResetTrigger Reset = WhenResetTrigger.Immediate;

        protected override bool OnCheckCondition(BTNode container)
        {
            if (Tree.TryGetTrigger(TriggerName, out var eventData))
            {
                if (Reset == WhenResetTrigger.Immediate)
                {
                    Tree.ResetTrigger(TriggerName);
                }
                return true;
            }
            return false;
        }

        public string GetDetail()
        {
            return @$"Name: ""{(string)TriggerName}""";
        }

        public void BeforeNodeEnter(BTNode container)
        {
            if (Reset == WhenResetTrigger.EnterNode)
            {
                Tree.ResetTrigger(TriggerName);
            }
        }

        public Status AfterNodeExit(Status result, BTNode container)
        {
            if (Reset == WhenResetTrigger.LeaveNode)
            {
                Tree.ResetTrigger(TriggerName);
            }
            return result;
        }

        public void OnNodeAbort(BTNode container)
        {
            if (Reset == WhenResetTrigger.LeaveNode)
            {
                Tree.ResetTrigger(TriggerName);
            }
        }
    }
}

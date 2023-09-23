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
    public enum WhenResetTrigger
    {
        Immediate,
        EnterNode,
        LeaveNode,
    }

    /// <summary>
    /// 检查自定义触发器
    /// <seealso cref="SetTrigger"/>
    /// </summary>
    [HelpURL(URL.WikiDecorator + "CheckTrigger_Decorator")]
    [DisplayName("CheckTrigger")]
    [SerializationAlias("Megumin.AI.BehaviorTree.CheckTrigger")]
    public class CheckTrigger_Decorator : ConditionDecorator, IDetailable, IPreDecorator, IPostDecorator, IAbortDecorator
    {
        public RefVar_String TriggerName;
        public WhenResetTrigger Reset = WhenResetTrigger.Immediate;

        protected override bool OnCheckCondition(object options = null)
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

        public void BeforeNodeEnter(object options = null)
        {
            if (Reset == WhenResetTrigger.EnterNode)
            {
                Tree.ResetTrigger(TriggerName);
            }
        }

        public Status AfterNodeExit(Status result, object options = null)
        {
            if (Reset == WhenResetTrigger.LeaveNode)
            {
                Tree.ResetTrigger(TriggerName);
            }
            return result;
        }

        public void OnNodeAbort(object options = null)
        {
            if (Reset == WhenResetTrigger.LeaveNode)
            {
                Tree.ResetTrigger(TriggerName);
            }
        }
    }
}

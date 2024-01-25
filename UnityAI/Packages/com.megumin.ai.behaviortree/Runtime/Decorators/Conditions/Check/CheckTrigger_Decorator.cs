using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Megumin.Binding;
using Megumin.Reflection;
using UnityEngine;
using UnityEngine.Serialization;

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

        [FormerlySerializedAsAttribute("Reset")]
        public WhenResetTrigger ResetTrigger = WhenResetTrigger.Immediate;

        protected override bool OnCheckCondition(object options = null)
        {
            if (Tree.TryGetTrigger(TriggerName.Value, out var eventData))
            {
                if (ResetTrigger == WhenResetTrigger.Immediate)
                {
                    eventData.Use();
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
            if (ResetTrigger == WhenResetTrigger.EnterNode)
            {
                Tree.RemoveTrigger(TriggerName.Value);
            }
        }

        public Status AfterNodeExit(Status result, object options = null)
        {
            if (ResetTrigger == WhenResetTrigger.LeaveNode)
            {
                Tree.RemoveTrigger(TriggerName.Value);
            }
            return result;
        }

        public void OnNodeAbort(object options = null)
        {
            if (ResetTrigger == WhenResetTrigger.LeaveNode)
            {
                Tree.RemoveTrigger(TriggerName.Value);
            }
        }
    }
}

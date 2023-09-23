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
    /// <summary>
    /// 检查超时
    /// </summary>
    [DisplayName("CheckTimeout")]
    [SerializationAlias("Megumin.AI.BehaviorTree.CheckTimeout")]
    [HelpURL(URL.WikiDecorator + "CheckTimeout_Decorator")]
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

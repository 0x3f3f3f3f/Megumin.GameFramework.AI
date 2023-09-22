using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Megumin.Binding;
using UnityEngine;

namespace Megumin.AI.BehaviorTree
{
    /// <summary>
    /// 设置触发器
    /// 与<see cref="CheckTrigger_Decorator"/>组合使用
    /// </summary>
    /// <remarks>
    /// 触发器生命周期为永久，但仅能被使用一次。
    /// </remarks>
    [Category("Action")]
    [HelpURL(URL.WikiTask + "SetTrigger")]
    public class SetTrigger : BTActionNode, IDetailable
    {
        public RefVar_String TriggerName;
        protected override Status OnTick(BTNode from, object options = null)
        {
            Tree.SetTrigger(TriggerName, this);
            return Status.Succeeded;
        }

        public string GetDetail()
        {
            return @$"Set ""{TriggerName?.Value}""";
        }
    }
}

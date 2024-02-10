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
    /// 发出事件
    /// 与<see cref="CheckEvent_Decorator"/>组合使用
    /// </summary>
    /// <remarks>
    /// 发出一个事件，根据名字，触发对应的事件检查节点。  
    ///事件的生命周期为一个tick。  
    ///事件可以同时触发多个事件检查节点。  
    /// </remarks>
    [Category("Action")]
    [HelpURL(URL.WikiTask + "SendEvent")]
    public class SendEvent : BTActionNode, IDetailable
    {
        public RefVar_String EventName;
        protected override Status OnTick(BTNode from, object options = null)
        {
            Tree.SendEvent(EventName.Value, sendNode: this);
            return Status.Succeeded;
        }

        public string GetDetail()
        {
            return @$"Send ""{EventName?.Value}"".";
        }
    }
}

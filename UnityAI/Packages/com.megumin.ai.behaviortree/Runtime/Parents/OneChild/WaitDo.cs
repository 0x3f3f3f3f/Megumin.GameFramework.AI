using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Megumin.Binding;
using Megumin.Timers;
using UnityEngine;

namespace Megumin.AI.BehaviorTree
{
    /// <summary>
    /// 等待指定时间，然后执行子节点
    /// </summary>
    [Icon("d_unityeditor.animationwindow@2x")]
    [DisplayName("WaitDo")]
    [HelpURL(URL.WikiTask + "WaitDo")]
    public class WaitDo : TimerParent
    {
        protected override void OnEnter(object options = null)
        {
            enterChild = false;
            base.OnEnter(options);
        }

        protected bool enterChild = false;
        protected override Status OnTick(BTNode from, object options = null)
        {
            if (enterChild)
            {
                return Child0.Tick(this, options);
            }
            else
            {
                if (WaitTimeable.WaitEnd(WaitTime))
                {
                    if (Child0 != null)
                    {
                        enterChild = true;
                    }
                    else
                    {
                        return Status.Succeeded;
                    }
                }
            }

            return Status.Running;
        }
    }
}

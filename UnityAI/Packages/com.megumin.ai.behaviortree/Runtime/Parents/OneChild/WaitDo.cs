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
    public class WaitDo : TimerParent, IDetailable
    {
        protected override void OnEnter(BTNode from, object options = null)
        {
            enterChild = false;
            base.OnEnter(from, options);
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

        public override string GetDetail()
        {
            if (State == Status.Running)
            {
                if (enterChild)
                {
                    return null;
                }
                else
                {
                    double left = WaitTimeable.GetLeftTime(WaitTime);
                    if (left >= 0)
                    {
                        return $"Wait: {(float)WaitTime:0.000}  Left:{left:0.000}";
                    }
                }
            }

            return $"Wait: {(float)WaitTime:0.000}";
        }
    }
}

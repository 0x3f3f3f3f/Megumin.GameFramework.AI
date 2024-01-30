using System.Collections;
using System.Collections.Generic;
using Megumin.Binding;
using Megumin.Timers;
using UnityEngine;

namespace Megumin.AI.BehaviorTree
{
    /// <summary>
    /// 
    /// </summary>
    [Icon("d_unityeditor.animationwindow@2x")]
    public abstract class TimerParent : OneChildNode, IDetailable
    {
        [Space]
        public UntiyTimeType TimeType = UntiyTimeType.GameTime;
        public RefVar_Float WaitTime = new() { value = 5.0f };

        internal protected IWaitTimeable<double> WaitTimeable;

        protected override void OnEnter(BTNode from, object options = null)
        {
            base.OnEnter(from, options);

            switch (TimeType)
            {
                case UntiyTimeType.GameTime:
                    WaitTimeable = new WaitGameTime();
                    break;
                case UntiyTimeType.UnscaledTime:
                    WaitTimeable = new WaitUnscaledTime();
                    break;
                case UntiyTimeType.Realtime:
                    WaitTimeable = new WaitRealtime();
                    break;
                default:
                    WaitTimeable = new WaitGameTime();
                    break;
            }

            WaitTimeable.WaitStart();
        }

        public virtual string GetDetail()
        {
            if (State == Status.Running)
            {
                double left = WaitTimeable.GetLeftTime(WaitTime);
                if (left >= 0)
                {
                    return $"Wait: {(float)WaitTime:0.000}  Left:{left:0.000}";
                }
            }

            return $"Wait: {(float)WaitTime:0.000}";
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Megumin.Binding;
using Megumin.Timers;
using UnityEngine;

namespace Megumin.AI.BehaviorTree
{
    /// <summary>
    /// 等待节点
    /// 等待一定时间后，返回成功。
    /// </summary>
    [Category("Action")]
    [Icon("d_unityeditor.animationwindow@2x")]
    [HelpURL(URL.WikiTask + "Wait")]
    public class Wait : BTActionNode, IDetailable
    {
        public UntiyTimeType TimeType = UntiyTimeType.GameTime;
        public RefVar_Float WaitTime = new() { value = 5.0f };

        private IWaitTimeable<double> WaitTimeable;

        //不要在Wait节点增加返回值设置，默认都是返回成功。
        //没有使用public CompletedResult Result = CompletedResult.Succeeded;
        //也没有使用GetIgnoreResult(from);
        //如果有的Wait返回成功，有的Wait返回失败，相同的节点返回不同返回值，
        //在UI上无法表现出Wait节点的差异性，会给用户造成困惑，非常不容理解。

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

        protected override Status OnTick(BTNode from, object options = null)
        {
            return WaitTimeable.WaitEnd(WaitTime) ? Status.Succeeded : Status.Running;
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

    public class WaitAction<T> : BTActionNode<T>, IDetailable
    {
        public UntiyTimeType TimeType = UntiyTimeType.GameTime;
        public RefVar_Float WaitTime = new() { value = 5.0f };

        private IWaitTimeable<double> WaitTimeable;

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

        protected override Status OnTick(BTNode from, object options = null)
        {
            return WaitTimeable.WaitEnd(WaitTime) ? Status.Succeeded : Status.Running;
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


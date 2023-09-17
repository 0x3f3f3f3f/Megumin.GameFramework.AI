﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Megumin.Binding;
using Megumin.Timers;
using UnityEngine;

namespace Megumin.AI.BehaviorTree
{
    [Category("Action")]
    [Icon("d_unityeditor.animationwindow@2x")]
    public class Wait : BTActionNode, IDetailable
    {
        public UntiyTimeType TimeType = UntiyTimeType.GameTime;
        public RefVar_Float WaitTime = new() { value = 5.0f };

        private IWaitTimeable<double> WaitTimeable;

        //不要在Wait节点增加返回值设置，默认都是返回成功。
        //因为在UI上无法表现出Wait节点的差异性，
        //有的Wait返回成功，有的Wait返回失败，相同的节点返回不同返回值，会给用户造成困惑。
        //public CompletedResult Result = CompletedResult.Succeeded;

        protected override void OnEnter(object options = null)
        {
            base.OnEnter(options);

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

        protected override void OnEnter(object options = null)
        {
            base.OnEnter(options);

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


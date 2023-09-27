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
    /// 简单跟随
    /// </summary>
    [Icon("buildsettings.android@2x")]
    [DisplayName("Follow")]
    [Description("Follow IMoveInputable<Vector3>")]
    [Category("Gameplay")]
    [AddComponentMenu("Follow Transform(IMoveInputable<Vector3>)")]
    [HelpURL(URL.WikiTask + "Follow")]
    public class Follow : OneChildNode<IMoveInputable<Vector3>>, IOutputPortInfoy<string>, IDetailable
    {
        public bool IgnoreYAxis = true;


        /// <summary>
        /// 最大停止距离
        /// </summary>
        public float nearCheck = 0.8f;

        /// <summary>
        /// 触发再次跟随移动距离
        /// </summary>
        public float farCheck = 2.5f;

        /// <summary>
        /// 跟随停止后到再次移动的间隔时间，防止抽搐
        /// </summary>
        public float nextMoveDelta = 1.5f;

        public RefVar_GameObject Target;

        protected override void OnEnter(object options = null)
        {
            base.OnEnter(options);
            InChild = false;
            LostMode = false;
            if (Target?.Value)
            {
                Last = Target.Value.transform.position;
            }
            else
            {
                //没有目标直接返回失败
                State = Status.Failed;
            }
        }

        protected Vector3 Last;
        /// <summary>
        /// 当前与目标的距离
        /// </summary>
        protected float CurrentDistance;

        /// <summary>
        /// 是否在子节点模式
        /// </summary>
        protected bool InChild = false;

        /// <summary>
        /// 开始下一次移动的间隔计时器
        /// </summary>
        readonly WaitGameTime NextMoveWait = new WaitGameTime();

        /// <summary>
        /// 丢失目标模式
        /// </summary>
        protected bool LostMode = false;

        /// <summary>
        /// 丢失目标计时器
        /// </summary>
        readonly WaitGameTime LostWait = new WaitGameTime();

        protected override Status OnTick(BTNode from, object options = null)
        {
            //这里的逻辑可以看作一个小型的状态机


            if (Target?.Value)
            {
                Last = Target.Value.transform.position;
                LostMode = false;
            }
            else
            {
                //跟随目标已经丢失
                if (LostMode)
                {
                    if (LostWait.WaitEnd(5f))
                    {
                        //丢失目标一定时间，返回失败
                        if (InChild)
                        {
                            Child0?.Abort(this);
                        }
                        return Status.Failed;
                    }
                }
                else
                {
                    //切换为丢失模式，启动计时器
                    LostMode = true;
                    LostWait.WaitStart();
                }
            }

            if (InChild)
            {
                //执行子节点
                CurrentDistance = (Transform.position - Last).magnitude;
                if (NextMoveWait.WaitEnd(nextMoveDelta) && CurrentDistance > farCheck)
                {
                    //转为跟随模式
                    InChild = false;
                    Child0?.Abort(this);
                    return Status.Running;
                }

                //在目标附近，执行子节点
                var childResult = Child0?.Tick(this, options);
                if (childResult == Status.Running)
                {
                    return Status.Running;
                }

                //子节点完成，返回跟随模式
                InChild = false;
                return Status.Running;
            }


            if (Transform.IsArrive(Last, out CurrentDistance, nearCheck, IgnoreYAxis))
            {
                //跟随足够接近目标，转为执行子节点。
                GetLogger()?.WriteLine($"MoveTo Succeeded: {Last}");
                MyAgent.MoveInput(Vector3.zero);
                InChild = true;
                NextMoveWait.WaitStart();
                return Status.Running;
            }
            else
            {
                //跟随移动，设置移动方向
                var dir = Last - Transform.position;
                MyAgent.MoveInput(dir);
            }

            return Status.Running;
        }

        public string OutputPortInfo => "OnNearTarget";

        public string GetDetail()
        {
            if (State == Status.Running)
            {
                if (InChild)
                {
                    return $"{CurrentDistance}--Wait:{NextMoveWait.GetLeftTime(nextMoveDelta):0.000}";
                }
                return $"{CurrentDistance}";
            }

            return null;
        }
    }
}



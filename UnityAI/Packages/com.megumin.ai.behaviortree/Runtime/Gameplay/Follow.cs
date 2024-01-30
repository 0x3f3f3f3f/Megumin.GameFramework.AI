using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Megumin.Binding;
using Megumin.Timers;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Pool;

namespace Megumin.AI.BehaviorTree
{
    /// <summary>
    /// 简单跟随，永远不会返回成功，会持续运行。
    /// </summary>

    public abstract class FollowBase<T> : OneChildNode<T>, IOutputPortInfoy<string>, IDetailable
    {
        [Space]
        public ArriveChecker ArriveChecker = new();

        /// <summary>
        /// 触发再次跟随移动距离
        /// </summary>
        public float RefollowDistance = 2.5f;

        /// <summary>
        /// 跟随停止后到再次移动的间隔时间，防止抽搐
        /// </summary>
        public float RefollowWait = 1.5f;

        /// <summary>
        /// 丢失目标后等待时间
        /// </summary>
        public float LostWait = 5f;

        public RefVar_GameObject Target;


        protected override void OnEnter(object options = null)
        {
            base.OnEnter(options);
            InChild = false;
            LostMode = false;
            if (Target?.Value)
            {
                Last = Target.Value.transform.position;
                ArriveChecker.CalStopingDistance(GameObject, Target.Value);
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
        protected readonly WaitGameTime RefollowWaiter = new WaitGameTime();

        /// <summary>
        /// 丢失目标模式
        /// </summary>
        protected bool LostMode = false;

        /// <summary>
        /// 丢失目标计时器
        /// </summary>
        protected readonly WaitGameTime LostWaiter = new WaitGameTime();

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
                    if (LostWaiter.WaitEnd(LostWait))
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
                    LostWaiter.WaitStart();
                }
            }

            if (InChild)
            {
                //执行子节点
                CurrentDistance = (Transform.position - Last).magnitude;
                if (RefollowWaiter.WaitEnd(RefollowWait) && CurrentDistance > RefollowDistance)
                {
                    //转为跟随模式
                    InChild = false;
                    Child0?.Abort(this);
                    return Status.Running;
                }

                if (Child0 == null)
                {
                    //没有子节点，暂时处于等待模式，防止立刻进行Refollow.
                    return Status.Running;
                }

                //在目标附近，执行子节点
                var childResult = Child0.Tick(this, options);
                if (childResult == Status.Running)
                {
                    return Status.Running;
                }

                //子节点完成，返回跟随模式。子节点的执行结果忽略，对Follow节点不造成影响。
                InChild = false;
                return Status.Running;
            }

            if (ArriveChecker.IsArrive(Transform, Last, out CurrentDistance))
            {
                //跟随足够接近目标，转为执行子节点。
                GetLogger()?.WriteLine($"MoveTo Succeeded: {Last}");
                OnArrivedTarget();
                InChild = true;
                RefollowWaiter.WaitStart();
            }
            else
            {
                OnFollowingTarget();
            }

            return Status.Running;
        }

        protected abstract void OnFollowingTarget();

        protected abstract void OnArrivedTarget();

        public string OutputPortInfo => "OnNearTarget";

        public string GetDetail()
        {
            if (State == Status.Running)
            {
                if (LostMode)
                {
                    return $"Lost:{LostWaiter.GetLeftTime(LostWait):0.000}";
                }

                if (InChild)
                {
                    var left = RefollowWaiter.GetLeftTime(RefollowWait);
                    if (left > 0)
                    {
                        return $"{CurrentDistance:0.000}  Wait:{left:0.000}";
                    }
                }

                return $"{CurrentDistance:0.000}";
            }

            return null;
        }
    }

    [Icon("buildsettings.android@2x")]
    [DisplayName("Follow (Dir)")]
    [Description("Follow IMoveInputable<Vector3>")]
    [Category("Gameplay")]
    [AddComponentMenu("Follow GameObject(IMoveInputable<Vector3>)")]
    [HelpURL(URL.WikiTask + "Follow")]
    [SerializationAlias("Megumin.AI.BehaviorTree.Follow")]
    public class Follow_MoveInput : FollowBase<IMoveInputable<Vector3>>
    {
        protected override void OnFollowingTarget()
        {
            //跟随移动，设置移动方向
            var dir = Last - Transform.position;
            MyAgent.MoveInput(dir, ArriveChecker, ArriveChecker.DistanceScale);
        }

        protected override void OnArrivedTarget()
        {
            MyAgent.MoveInput(Vector3.zero, ArriveChecker, ArriveChecker.DistanceScale);
        }
    }

    [Icon("buildsettings.android@2x")]
    [DisplayName("Follow (Pos)")]
    [Description("Follow IMoveToable<Vector3>")]
    [Category("Gameplay")]
    [AddComponentMenu("Follow GameObject(IMoveToable<Vector3>)")]
    [HelpURL(URL.WikiTask + "Follow")]
    public class Follow_MoveTo : FollowBase<IMoveToable<Vector3>>
    {
        protected override void OnFollowingTarget()
        {
            MyAgent.MoveTo(Last, ArriveChecker, ArriveChecker.DistanceScale);
        }

        protected override void OnArrivedTarget()
        {

        }
    }

    [Icon("buildsettings.android@2x")]
    [DisplayName("Follow")]
    [Description("Follow NavMeshAgent.SetDestination")]
    [Category("Gameplay")]
    [AddComponentMenu("Follow GameObject(NavMeshAgent.SetDestination)")]
    [HelpURL(URL.WikiTask + "Follow")]
    public class Follow_NavAgent : FollowBase<NavMeshAgent>
    {
        protected override void OnFollowingTarget()
        {
            MyAgent.SetDestination(Last);
            MyAgent.stoppingDistance = ArriveChecker;
        }

        protected override void OnArrivedTarget()
        {

        }
    }

}



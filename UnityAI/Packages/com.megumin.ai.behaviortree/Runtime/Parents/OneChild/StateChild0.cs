using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Megumin.AI.BehaviorTree
{
    /// <summary>
    /// 根据特定状态执行Child0。看作一个小状态机。
    /// <para/>
    /// 近似装饰节点DecoratorNode，是一种特殊的OneChildNode。
    /// 但称为装饰节点并不合适，因为不是以修饰子节点为主要目的的。
    /// 它自身的逻辑可能不通用，并且同样十分主要，不能看作一个装饰。
    /// </summary>
    public abstract class StateChild0 : OneChildNode
    {
        protected override void OnEnter(object options = null)
        {
            base.OnEnter(options);
            InChild = false;
        }

        protected bool InChild = false;
        protected override Status OnTick(BTNode from, object options = null)
        {
            if (InChild)
            {
                //执行子节点
                var childResult = Child0?.Tick(this, options);
                if (childResult == Status.Running)
                {
                    return Status.Running;
                }

                //子节点完成，由子类决定继续执行还是完成。
                InChild = false;
                return OnChildComplete(childResult);
            }
            else
            {
                //自身执行部分，决定是否进入子节点，还是完成自身。
                var (ChangeTo, Result) = OnTickSelf(from, options);
                if (ChangeTo)
                {
                    InChild = true;
                    return Status.Running;
                }
                else
                {
                    return Result;
                }
            }
        }

        /// <summary>
        /// 父节点自身的逻辑，决定是否进入子节点，还是完成自身。
        /// </summary>
        /// <param name="from"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public abstract (bool ChangeTo, Status Result) OnTickSelf(BTNode from, object options = null);

        /// <summary>
        /// 子节点为null或者完成时被调用。
        /// 由子类决定继续执行还是完成。
        /// </summary>
        /// <param name="childResult"></param>
        /// <returns></returns>
        public abstract Status OnChildComplete(Status? childResult);
    }



    /// <summary>
    /// 根据特定状态执行Child0。看作一个小状态机。
    /// <para/>
    /// 近似装饰节点DecoratorNode，是一种特殊的OneChildNode。
    /// 但称为装饰节点并不合适，因为不是以修饰子节点为主要目的的。
    /// 它自身的逻辑可能不通用，并且同样十分主要，不能看作一个装饰。
    /// </summary>
    public abstract class StateChild0<T> : OneChildNode<T>
    {
        protected override void OnEnter(object options = null)
        {
            base.OnEnter(options);
            InChild = false;
        }

        protected bool InChild = false;
        protected override Status OnTick(BTNode from, object options = null)
        {
            if (InChild)
            {
                //执行子节点
                var childResult = Child0?.Tick(this, options);
                if (childResult == Status.Running)
                {
                    return Status.Running;
                }

                //子节点完成，由子类决定继续执行还是完成。
                InChild = false;
                return OnChildComplete(childResult);
            }
            else
            {
                //自身执行部分，决定是否进入子节点，还是完成自身。
                var (ChangeTo, Result) = OnTickSelf(from, options);
                if (ChangeTo)
                {
                    InChild = true;
                    return Status.Running;
                }
                else
                {
                    return Result;
                }
            }
        }

        /// <summary>
        /// 父节点自身的逻辑，决定是否进入子节点，还是完成自身。
        /// </summary>
        /// <param name="from"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public abstract (bool ChangeTo, Status Result) OnTickSelf(BTNode from, object options = null);

        /// <summary>
        /// 子节点为null或者完成时被调用。
        /// 由子类决定继续执行还是完成。
        /// </summary>
        /// <param name="childResult"></param>
        /// <returns></returns>
        public abstract Status OnChildComplete(Status? childResult);
    }



    [Obsolete("失败设计，当otherNode为null时，OnChildComp无法被调用", true)]
    public abstract class PassThrough : OneChildNode
    {
        protected override void OnEnter(object options = null)
        {
            base.OnEnter(options);
            otherNode = null;
        }

        protected BTTaskNode otherNode = null;
        //protected override Status OnTick(BTNode from, object options = null)
        //{
        //    if (otherNode != null)
        //    {
        //        //执行子节点
        //        var childResult = Child0?.Tick(this, options);
        //        if (childResult == Status.Running)
        //        {
        //            return Status.Running;
        //        }

        //        //子节点完成，由子类决定继续执行还是完成。
        //        otherNode = null;
        //        return OnChildComp(childResult);
        //    }
        //    else
        //    {
        //        //自身执行部分，决定是否进入子节点，还是完成自身。
        //        var (ChangeTo, Result) = OnTickSelf(from, options);
        //        if (ChangeTo != null)
        //        {
        //            InChild = true;
        //            return Status.Running;
        //        }
        //        else
        //        {
        //            return Result;
        //        }
        //    }
        //}

        public abstract (BTTaskNode otherNode, Status Result) OnTickSelf(BTNode from, object options = null);
        /// <summary>
        /// 子节点为null或者完成时被调用。
        /// 由子类决定继续执行还是完成。
        /// </summary>
        /// <param name="childResult"></param>
        /// <returns></returns>
        public abstract Status OnOtherNode(Status? childResult);
    }
}




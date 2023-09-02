using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Megumin.Binding;
using UnityEngine;

namespace Megumin.AI.BehaviorTree
{
    public interface IMoveToVector3able
    {
        void MoveTo(Vector3 destination);
    }

    public abstract class MoveToBase<T> : BTActionNode<T>
    {
        [Space]
        public float StopingDistance = 0.25f;

        public bool IgnoreYAxis = false;


        /// <summary>
        /// 移动过程中目的地改变，自动重新设置目的地
        /// </summary>
        [Space]
        public bool KeepDestinationNew = false;

        protected Vector3 Last;

        protected override void OnEnter(object options = null)
        {
            InternalMoveTo();
        }

        protected abstract void InternalMoveTo();
        protected abstract Vector3 GetDestination();

        protected override Status OnTick(BTNode from, object options = null)
        {
            if (KeepDestinationNew)
            {
                if (Last != GetDestination())
                {
                    InternalMoveTo();
                }
            }

            var current = Transform.position;
            var destination = Last;

            if (IgnoreYAxis)
            {
                //忽略Y轴。
                current.y = 0;
                destination.y = 0;
            }

            var distance = Vector3.Distance(current, destination);

            if (distance > StopingDistance)
            {
                return Status.Running;
            }

            return Status.Succeeded;
        }
    }

    [Icon("d_navmeshdata icon")]
    [DisplayName("MoveTo")]
    [Category("Gameplay")]
    [AddComponentMenu("MoveTo(Vector3)")]
    public sealed class MoveToVector3 : MoveToBase<IMoveToVector3able>
    {
        [Space]
        public Destination destination;

        protected override void InternalMoveTo()
        {
            Last = GetDestination();
            MyAgent.MoveTo(Last);
        }

        protected override Vector3 GetDestination()
        {
            return destination.GetDestination();
        }
    }
}



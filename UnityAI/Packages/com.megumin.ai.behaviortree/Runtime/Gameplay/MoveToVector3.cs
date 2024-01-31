using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Megumin.Binding;
using UnityEngine;

namespace Megumin.AI.BehaviorTree
{
    public abstract class MoveToBase<T> : BTActionNode<T>
    {
        [Space]
        public Destination destination;

        [Space]
        public ArriveChecker ArriveChecker = new();

        /// <summary>
        /// 移动过程中目的地改变，自动重新设置目的地
        /// </summary>
        [Space]
        public bool KeepDestinationNew = false;

        protected Vector3 Last;

        protected override void OnEnter(BTNode from, object options = null)
        {
            ArriveChecker.CalStopingDistance(GameObject, destination.Target);
            InternalMoveTo();
        }

        protected abstract void InternalMoveTo();
        protected virtual Vector3 GetDestination()
        {
            return destination.GetDestination();
        }

        protected override Status OnTick(BTNode from, object options = null)
        {
            if (KeepDestinationNew)
            {
                if (Last != GetDestination())
                {
                    InternalMoveTo();
                }
            }

            if (ArriveChecker.IsArrive(Transform, Last))
            {
                GetLogger()?.WriteLine($"MoveTo Succeeded: {Last}");
                return Status.Succeeded;
            }

            return Status.Running;
        }
    }

    [Icon("d_navmeshdata icon")]
    [DisplayName("MoveTo (Pos)")]
    [Description("IMoveToable<Vector3>")]
    [Category("Gameplay")]
    [AddComponentMenu("MoveTo(IMoveToable<Vector3>)")]
    [HelpURL(URL.WikiTask + "MoveToVector3")]
    public sealed class MoveToVector3 : MoveToBase<IMoveToable<Vector3>>
    {
        protected override void InternalMoveTo()
        {
            Last = GetDestination();
            MyAgent.MoveTo(Last, ArriveChecker, ArriveChecker.DistanceScale);

            GetLogger()?.WriteLine($"MoveTo MyAgent : {MyAgent}  Des : {destination?.Dest_Transform?.Value.name} Last:{Last}");
        }
    }
}



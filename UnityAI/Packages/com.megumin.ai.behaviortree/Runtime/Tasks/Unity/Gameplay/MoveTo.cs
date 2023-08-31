using System.Collections;
using System.Collections.Generic;
using Megumin.Binding;
using UnityEngine;

namespace Megumin.AI.BehaviorTree
{
    public interface IMoveable
    {
        void MoveTo(Vector3 destination);
    }

    public class MoveTo : BTActionNode<IMoveable>
    {
        public RefVar_Vector3 Destination;

        public float StopingDistance = 0.25f;
        public bool IgnoreYAxisDistance = false;
        public bool KeepDestinationNew = false;

        Vector3 last;
        protected override void OnEnter(object options = null)
        {
            InternalMoveTo();
        }

        private void InternalMoveTo()
        {
            last = Destination;
            MyAgent.MoveTo(last);
        }

        protected override Status OnTick(BTNode from, object options = null)
        {
            if (KeepDestinationNew)
            {
                if (last != Destination)
                {
                    InternalMoveTo();
                }
            }

            var current = Transform.position;
            var destination = last;

            if (IgnoreYAxisDistance)
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
}

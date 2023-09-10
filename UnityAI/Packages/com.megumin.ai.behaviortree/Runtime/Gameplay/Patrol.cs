using System.Collections;
using System.Collections.Generic;
using Megumin.Binding;
using UnityEngine;

namespace Megumin.AI.BehaviorTree
{
    /// <summary>
    /// 可巡逻的
    /// </summary>
    public interface IPatrolable : IMoveToable<Vector3>
    {
    }

    public class Patrol : BTActionNode<IPatrolable>
    {
        [Space]
        public float StopingDistance = 0.25f;

        public bool IgnoreYAxis = false;
        public RefVar_Transform_List DestinationList;


        Transform lastDes;
        int lastDesIndex = 0;
        protected override Status OnTick(BTNode from, object options = null)
        {
            if (lastDes == null && TryMoveNext() == false)
            {
                return Status.Failed;
            }

            if (Transform.IsArrive(lastDes.position, StopingDistance, IgnoreYAxis))
            {
                if (TryMoveNext())
                {
                    return Status.Running;
                }
                else
                {
                    return Status.Succeeded;
                }
            }

            return Status.Running;
        }

        public bool TryMoveNext()
        {
            if (!lastDes)
            {

                var list = DestinationList?.Value;
                if (list != null && list.Count != 0)
                {
                    lastDes = list[lastDesIndex % list.Count].transform;
                    lastDesIndex++;
                    return MyAgent.MoveTo(lastDes.position);
                }
            }

            if (!lastDes)
            {
                return false;
            }

            return true;
        }
    }
}


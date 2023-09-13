using System.Collections;
using System.Collections.Generic;
using Megumin.Binding;
using UnityEngine;

namespace Megumin.AI.BehaviorTree
{
    /// <summary>
    /// 巡逻节点
    /// </summary>
    public class Patrol_1 : BTActionNode<IMoveToable<Vector3>>
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

    public class Patrol_2 : BTActionNode<IMoveToable<Vector3>>
    {
        [Space]
        public float StopingDistance = 0.25f;

        public bool IgnoreYAxis = false;

        [Space]
        public float MaxRange = 10f;
        public float MinRange = 2f;

        [Space]
        public float MinDistance2Current = 4f;

        Vector3 start;

        protected override void OnEnter(object options = null)
        {
            start = Transform.position;
            TryMoveNext();
        }

        Vector3 lastDes;
        protected override Status OnTick(BTNode from, object options = null)
        {
            if (Transform.IsArrive(lastDes, StopingDistance, IgnoreYAxis))
            {
                //IDEL
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
            Vector3 next = RandomNext(start);

            if (MyAgent.MoveTo(next))
            {
                lastDes = next;
                return true;
            }
            return false;
        }

        public Vector3 RandomNext(Vector3 orignal)
        {
            int count = 0;
            while (true)
            {
                var random = Random.insideUnitCircle * Random.Range(MinRange, MaxRange);
                var next = orignal + new Vector3(random.x, 0, random.y);
                count++;
                if ((Transform.position - next).magnitude > MinDistance2Current || count > 20)
                {
                    //随机的下一个点 必须与当前位置 距离足够远
                    return next;
                }
            }
        }
    }
}


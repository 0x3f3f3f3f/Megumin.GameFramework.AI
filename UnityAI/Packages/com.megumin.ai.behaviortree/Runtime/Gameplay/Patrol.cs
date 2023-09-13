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

        public bool IgnoreYAxis = true;
        public RefVar_Transform_List DestinationList;

        /// <summary>
        /// 每次获取新的目标点时，是否随机便宜一段距离
        /// </summary>
        [Space]
        public bool UseRandom = false;
        public float MaxOffset = 2;
        public float MinOffset = 0.5f;

        Vector3 startPosition;
        protected override void OnEnter(object options = null)
        {
            startPosition = Transform.position;
            TryMoveNext();
        }

        Vector3 lastDestination;
        protected override Status OnTick(BTNode from, object options = null)
        {
            if (Transform.IsArrive(lastDestination, StopingDistance, IgnoreYAxis))
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
            if (TryGetNext(out var next))
            {
                if (MyAgent.MoveTo(next))
                {
                    lastDestination = next;
                    return true;
                }
            }

            return false;
        }

        int lastDesIndex = 0;
        public bool TryGetNext(out Vector3 next)
        {
            var list = DestinationList?.Value;
            if (list != null && list.Count != 0)
            {
                int startIndex = lastDesIndex % list.Count;
                for (int i = startIndex; i < list.Count; i++)
                {
                    lastDesIndex++;
                    Transform transform = list[i].transform;
                    if (transform)
                    {
                        next = transform.position;
                        if (UseRandom)
                        {
                            var offset = Random.insideUnitCircle * Random.Range(MinOffset, MaxOffset);
                            next += new Vector3(offset.x, 0, offset.y);
                        }
                        return true;
                    }
                }
            }
            next = default;
            return false;
        }
    }

    public class Patrol_2 : BTActionNode<IMoveToable<Vector3>>
    {
        [Space]
        public float StopingDistance = 0.25f;

        public bool IgnoreYAxis = true;

        [Space]
        public float MaxRange = 10f;
        public float MinRange = 2f;

        [Space]
        public float MinDistance2Current = 4f;

        Vector3 startPosition;

        protected override void OnEnter(object options = null)
        {
            startPosition = Transform.position;
            TryMoveNext();
        }

        Vector3 lastDestination;
        protected override Status OnTick(BTNode from, object options = null)
        {
            if (Transform.IsArrive(lastDestination, StopingDistance, IgnoreYAxis))
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
            if (TryGetNext(out var next))
            {
                if (MyAgent.MoveTo(next))
                {
                    lastDestination = next;
                    return true;
                }
            }

            return false;
        }

        public bool TryGetNext(out Vector3 next)
        {
            int count = 0;
            while (true)
            {
                var random = Random.insideUnitCircle * Random.Range(MinRange, MaxRange);
                next = startPosition + new Vector3(random.x, 0, random.y);
                count++;
                if ((Transform.position - next).magnitude > MinDistance2Current || count > 20)
                {
                    //随机的下一个点 必须与当前位置 距离足够远
                    return true;
                }
            }
        }
    }
}


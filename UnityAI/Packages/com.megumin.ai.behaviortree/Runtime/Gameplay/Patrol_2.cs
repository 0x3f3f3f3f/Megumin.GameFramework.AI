using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

namespace Megumin.AI.BehaviorTree
{
    [Icon("d_navmeshdata icon")]
    [DisplayName("Patrol")]
    [Description("Random InsideCircle IMoveToable<Vector3>")]
    [Category("Gameplay")]
    [AddComponentMenu("Patrol Random InsideCircle(IMoveToable<Vector3>)")]
    [HelpURL(URL.WikiTask + "Patrol")]
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



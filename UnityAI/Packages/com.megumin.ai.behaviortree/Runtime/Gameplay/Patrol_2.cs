using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

namespace Megumin.AI.BehaviorTree
{
    /// <summary>
    /// 巡逻节点
    /// </summary>
    [Icon("d_navmeshdata icon")]
    [DisplayName("Patrol")]
    [Description("Random InsideCircle IMoveToable<Vector3>")]
    [Category("Gameplay")]
    [AddComponentMenu("Patrol Random InsideCircle(IMoveToable<Vector3>)")]
    [HelpURL(URL.WikiTask + "Patrol")]
    public class Patrol_2 : PatrolBase<IMoveToable<Vector3>>
    {
        [Space]
        public float MaxRange = 12f;
        public float MinRange = 4f;

        [Space]
        public float MinDistance2Current = 6f;

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

        public override bool TryMoveNext(ref Vector3 destination)
        {
            if (TryGetNext(out var next))
            {
                if (MyAgent.MoveTo(next))
                {
                    destination = next;
                    return true;
                }
            }

            return false;
        }
    }
}



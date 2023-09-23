using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Megumin.Binding;
using UnityEngine;

namespace Megumin.AI.BehaviorTree
{
    /// <summary>
    /// 巡逻基类
    /// 每到达一个检查点，执行一次子节点。
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class PatrolBase<T> : StateChild0<T>
    {
        [Space]
        public float StopingDistance = 0.25f;

        public bool IgnoreYAxis = true;

        /// <summary>
        /// 每次获取新的目标点时，是否随机便宜一段距离
        /// </summary>
        [Space]
        public bool UseRandom = false;
        public float MaxOffset = 2;
        public float MinOffset = 0.5f;

        protected Vector3 startPosition;
        protected override void OnEnter(object options = null)
        {
            base.OnEnter(options);
            startPosition = Transform.position;
            TryMoveNext(ref destination);
        }

        protected Vector3 destination;
        public override (bool ChangeTo, Status Result) OnTickSelf(BTNode from, object options = null)
        {
            if (Transform.IsArrive(destination, StopingDistance, IgnoreYAxis))
            {
                return (true, Status.Running);
            }

            return (false, Status.Running);
        }

        public override Status OnChildComplete(Status? childResult)
        {
            if (TryMoveNext(ref destination))
            {
                return Status.Running;
            }
            else
            {
                return Status.Succeeded;
            }
        }

        /// <summary>
        /// 尝试移动到下一个检查点
        /// </summary>
        /// <param name="destination">使用ref 而不是out，当移动失败时，不改变destination现有值</param>
        /// <returns></returns>
        public abstract bool TryMoveNext(ref Vector3 destination);
    }

    /// <summary>
    /// 巡逻节点
    /// </summary>
    [Icon("d_navmeshdata icon")]
    [DisplayName("Patrol")]
    [Description("Transform_List IMoveToable<Vector3>")]
    [Category("Gameplay")]
    [AddComponentMenu("Patrol Transform_List(IMoveToable<Vector3>)")]
    [HelpURL(URL.WikiTask + "Patrol")]
    public class Patrol_1 : PatrolBase<IMoveToable<Vector3>>
    {
        [Space]
        public RefVar_Transform_List DestinationList;
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


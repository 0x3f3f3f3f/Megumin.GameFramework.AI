using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Megumin.Binding;
using UnityEngine;

namespace Megumin.AI.BehaviorTree
{
    public abstract class Patrol_MoveTo<T, P> : PatrolBase<T, P>
        where T : IMoveToable<Vector3>
        where P : PatrolPath<Vector3>
    {
        protected override void OnEnter(object options = null)
        {
            base.OnEnter(options);
            if (TryMoveNext(ref destination))
            {

            }
            else
            {
                State = Status.Failed;
            }
        }

        public override (bool ChangeTo, Status Result) OnTickSelf(BTNode from, object options = null)
        {
            if (Transform.IsArrive(destination, StopingDistance, IgnoreYAxis))
            {
                PatrolPath.Arrive(destination);
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
        public virtual bool TryMoveNext(ref Vector3 destination)
        {
            if (PatrolPath.TryGetNextDestination(Transform, out var next))
            {
                if (MyAgent.MoveTo(next, StopingDistance))
                {
                    destination = next;
                    return true;
                }
            }
            return false;
        }
    }

    /// <summary>
    /// 巡逻节点
    /// </summary>
    [Icon("d_navmeshdata icon")]
    [DisplayName("Patrol_MoveTo (Pos)")]
    [Description("Transform_List IMoveToable<Vector3>")]
    [Category("Gameplay")]
    [AddComponentMenu("Patrol Transform_List(IMoveToable<Vector3>)")]
    [HelpURL(URL.WikiTask + "Patrol")]
    public class Patrol_1 : Patrol_MoveTo<IMoveToable<Vector3>, PatrolPathTranformList>
    {
    }

    /// <summary>
    /// 巡逻节点
    /// </summary>
    [Icon("d_navmeshdata icon")]
    [DisplayName("Patrol_MoveTo (Pos)")]
    [Description("Random InsideCircle IMoveToable<Vector3>")]
    [Category("Gameplay")]
    [AddComponentMenu("Patrol Random InsideCircle(IMoveToable<Vector3>)")]
    [HelpURL(URL.WikiTask + "Patrol")]
    public class Patrol_2 : Patrol_MoveTo<IMoveToable<Vector3>, PatrolPathAroundStart>
    {

    }
}

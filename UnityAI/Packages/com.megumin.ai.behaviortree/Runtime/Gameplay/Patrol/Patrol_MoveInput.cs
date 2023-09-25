using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

namespace Megumin.AI.BehaviorTree
{
    public abstract class Patrol_MoveInput<T, P> : PatrolBase<T, P>
        where T : IMoveInputable<Vector3>
        where P : PatrolPath<Vector3>
    {
        protected override void OnEnter(object options = null)
        {
            base.OnEnter(options);
            if (PatrolPath.TryGetNextDestination(Transform, out var next))
            {
                destination = next;
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
                MyAgent.MoveInput(Vector3.zero);
                return (true, Status.Running);
            }
            else
            {
                var dir = destination - Transform.position;
                MyAgent.MoveInput(dir);
            }

            return (false, Status.Running);
        }

        public override Status OnChildComplete(Status? childResult)
        {
            if (PatrolPath.TryGetNextDestination(Transform, out var next))
            {
                destination = next;
                return Status.Running;
            }
            else
            {
                return Status.Succeeded;
            }
        }
    }

    /// <summary>
    /// 巡逻节点
    /// </summary>
    [Icon("d_navmeshdata icon")]
    [DisplayName("Patrol_MoveInput")]
    [Description("Transform_List IMoveInputable<Vector3>")]
    [Category("Gameplay")]
    [AddComponentMenu("Patrol Transform_List(IMoveInputable<Vector3>)")]
    [HelpURL(URL.WikiTask + "Patrol")]
    public class Patrol_3 : Patrol_MoveInput<IMoveInputable<Vector3>, PatrolPathTranformList>
    {

    }

    /// <summary>
    /// 巡逻节点
    /// </summary>
    [Icon("d_navmeshdata icon")]
    [DisplayName("Patrol_MoveInput")]
    [Description("Random InsideCircle IMoveInputable<Vector3>")]
    [Category("Gameplay")]
    [AddComponentMenu("Patrol Random InsideCircle(IMoveInputable<Vector3>)")]
    [HelpURL(URL.WikiTask + "Patrol")]
    public class Patrol_4 : Patrol_MoveInput<IMoveInputable<Vector3>, PatrolPathAroundStart>
    {

    }
}

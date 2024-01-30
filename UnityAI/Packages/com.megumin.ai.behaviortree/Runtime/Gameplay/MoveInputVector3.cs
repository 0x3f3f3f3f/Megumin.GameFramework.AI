using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

namespace Megumin.AI.BehaviorTree
{
    [Icon("d_navmeshdata icon")]
    [DisplayName("MoveTo  (Dir)")]
    [Description("IMoveInputable<Vector3>")]
    [Category("Gameplay")]
    [AddComponentMenu("MoveTo(IMoveInputable<Vector3>)")]
    [HelpURL(URL.WikiTask + "MoveInputVector3")]
    public class MoveInputVector3 : BTActionNode<IMoveInputable<Vector3>>
    {
        [Space]
        public StopingDistance StopingDistance = new();

        public bool IgnoreYAxis = true;


        /// <summary>
        /// 移动过程中目的地改变，自动重新设置目的地
        /// </summary>
        [Space]
        public bool KeepDestinationNew = false;

        [Space]
        public Destination destination;

        protected Vector3 Last;

        protected override void OnEnter(object options = null)
        {
            base.OnEnter(options);
            StopingDistance.Cal(GameObject, null);
            Last = GetDestination();
            GetLogger()?.WriteLine($"MoveTo MyAgent : {MyAgent}  Des : {destination?.Dest_Transform?.Value.name} Last:{Last}");
        }

        protected Vector3 GetDestination()
        {
            return destination.GetDestination();
        }

        protected override Status OnTick(BTNode from, object options = null)
        {
            if (KeepDestinationNew)
            {
                Last = GetDestination();
            }

            if (Transform.IsArrive(Last, StopingDistance, IgnoreYAxis))
            {
                MyAgent.MoveInput(Vector3.zero, StopingDistance);
                GetLogger()?.WriteLine($"MoveTo Succeeded: {Last}");
                return Status.Succeeded;
            }
            else
            {
                var dir = Last - Transform.position;
                MyAgent.MoveInput(dir, StopingDistance);
            }

            return Status.Running;
        }
    }
}

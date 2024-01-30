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
        /// �ƶ�������Ŀ�ĵظı䣬�Զ���������Ŀ�ĵ�
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

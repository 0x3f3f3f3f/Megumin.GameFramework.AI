using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Megumin.Binding;
using UnityEngine;
using UnityEngine.AI;

namespace Megumin.AI.BehaviorTree
{
    [Icon("d_navmeshdata icon")]
    [DisplayName("MoveTo")]
    [Category("UnityEngine/NavMeshAgent")]
    [AddComponentMenu("MoveTo(SetDestination)")]
    public class MoveTo_SetDestination : MoveToBase<NavMeshAgent>
    {
        [Space]
        public Destination destination;

        protected override void InternalMoveTo()
        {
            Last = GetDestination();
            MyAgent.SetDestination(Last);
            this.Transform.LookAt(Last);
        }


        protected override Vector3 GetDestination()
        {
            return destination.GetDestination();
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Megumin.Binding;
using UnityEngine;

namespace Megumin.AI.BehaviorTree
{
    [Icon("d_navmeshdata icon")]
    [DisplayName("MoveTo")]
    [Category("Gameplay")]
    [AddComponentMenu("MoveTo(Transform)")]
    public class MoveToTranform : MoveToBase<IMoveToVector3able>
    {
        [Space]
        public RefVar_Transform Destination;

        protected override void InternalMoveTo()
        {
            Last = GetDestination();
            MyAgent.MoveTo(Last);
        }

        protected override Vector3 GetDestination()
        {
            return Destination.Value.position;
        }
    }
}





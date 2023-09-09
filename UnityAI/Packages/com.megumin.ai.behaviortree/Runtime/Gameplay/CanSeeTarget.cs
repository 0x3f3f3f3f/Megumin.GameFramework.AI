#if MEGUMIN_PERCEPTION

using System.Collections;
using System.Collections.Generic;
using Megumin.Binding;
using Megumin.AI;
using Megumin.AI.BehaviorTree;
using Megumin.Perception;
using UnityEngine;
using System.ComponentModel;

namespace Megumin.AI.BehaviorTree
{
    [Icon("d_viewtoolorbit on@2x")]
    [Description("TransformPerception CanSeeTarget ")]
    [Category("Gameplay")]
    [AddComponentMenu("CanSeeTarget(Transform)")]
    public class CanSeeTarget : ConditionDecorator<TransformPerception>
    {
        public RefVar_Transform Target;
        protected override bool OnCheckCondition(object options = null)
        {
            return Target.Value == MyAgent.AutoTarget;
        }
    }

}

#endif


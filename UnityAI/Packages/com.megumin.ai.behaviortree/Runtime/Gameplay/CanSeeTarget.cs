#if MEGUMIN_PERCEPTION

using System.Collections;
using System.Collections.Generic;
using Megumin.Binding;
using Megumin.AI;
using Megumin.AI.BehaviorTree;
using Megumin.Perception;
using UnityEngine;
using System.ComponentModel;
using UnityEngine.Serialization;
using Megumin.Serialization;
using Megumin.Reflection;

namespace Megumin.AI.BehaviorTree
{
    [Icon("d_viewtoolorbit on@2x")]
    [DisplayName("CanSeeTarget")]
    [Description("TransformPerception CanSeeTarget")]
    [Category("Gameplay")]
    [AddComponentMenu("CanSeeTarget(Transform)")]
    [SerializationAlias("CanSeeTarget")]
    [HelpURL(URL.WikiDecorator + "CanSeeTarget")]
    public class CanSeeTarget_Transform : ConditionDecorator<TransformPerception>
    {
        public RefVar_Transform Target;
        protected override bool OnCheckCondition(object options = null)
        {
            return Target.Value == MyAgent.AutoTarget;
        }
    }

    [Icon("d_viewtoolorbit on@2x")]
    [DisplayName("CanSeeTarget")]
    [Description("GameObjectPerception CanSeeTarget")]
    [Category("Gameplay")]
    [AddComponentMenu("CanSeeTarget(GameObject)")]
    public class CanSeeTarget_GameObject : ConditionDecorator<GameObjectPerception>
    {
        public RefVar_GameObject Target;
        protected override bool OnCheckCondition(object options = null)
        {
            return Target.Value == MyAgent.AutoTarget;
        }
    }
}

#endif


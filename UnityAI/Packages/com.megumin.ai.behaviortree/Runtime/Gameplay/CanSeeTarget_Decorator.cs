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
    [Category("Gameplay/Perception")]
    [AddComponentMenu("CanSeeTarget(Transform)")]
    [SerializationAlias("Megumin.AI.BehaviorTree.CanSeeTarget_Transform")]
    [HelpURL(URL.WikiDecorator + "CanSeeTarget_Transform_Decorator")]
    public class CanSeeTarget_Transform_Decorator : ConditionDecorator<TransformPerception>
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
    [Category("Gameplay/Perception")]
    [AddComponentMenu("CanSeeTarget(GameObject)")]
    [SerializationAlias("Megumin.AI.BehaviorTree.CanSeeTarget_GameObject")]
    [HelpURL(URL.WikiDecorator + "CanSeeTarget_GameObject_Decorator")]
    public class CanSeeTarget_GameObject_Decorator : ConditionDecorator<GameObjectPerception>
    {
        public RefVar_GameObject Target;
        protected override bool OnCheckCondition(object options = null)
        {
            return Target.Value == MyAgent.AutoTarget;
        }
    }
}

#endif


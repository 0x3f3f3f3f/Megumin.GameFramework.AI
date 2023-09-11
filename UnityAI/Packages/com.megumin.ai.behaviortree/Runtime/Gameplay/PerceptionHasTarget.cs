#if MEGUMIN_PERCEPTION

using System.ComponentModel;
using Megumin.Binding;
using Megumin.Perception;
using Megumin.Reflection;
using UnityEngine;

namespace Megumin.AI.BehaviorTree
{
    [Icon("d_viewtoolorbit on@2x")]
    [DisplayName("Has Target?")]
    [Description("TransformPerception Has Target?")]
    [Category("Gameplay/Perception")]
    [AddComponentMenu("HasTarget(Transform)")]
    [SerializationAlias("HasTarget")]
    [HelpURL(URL.WikiDecorator + "HasTarget")]
    public class PerceptionHasTarget_Transform : ConditionDecorator<TransformPerception>
    {
        public RefVar_Transform SaveTo;
        protected override bool OnCheckCondition(object options = null)
        {
            SaveTo.Value = MyAgent.AutoTarget;
            return MyAgent.AutoTarget;
        }
    }

    [Icon("d_viewtoolorbit on@2x")]
    [DisplayName("Has Target?")]
    [Description("GameObjectPerception Has Target?")]
    [Category("Gameplay/Perception")]
    [AddComponentMenu("HasTarget(GameObject)")]
    [HelpURL(URL.WikiDecorator + "HasTarget")]
    public class PerceptionHasTarget_GameObject : ConditionDecorator<GameObjectPerception>
    {
        public RefVar_GameObject SaveTo;
        protected override bool OnCheckCondition(object options = null)
        {
            SaveTo.Value = MyAgent.AutoTarget;
            return MyAgent.AutoTarget;
        }
    }
}

#endif


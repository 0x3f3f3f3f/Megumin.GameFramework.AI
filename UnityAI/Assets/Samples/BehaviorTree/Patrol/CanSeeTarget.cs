using System.Collections;
using System.Collections.Generic;
using Megumin.Binding;
using Megumin.AI;
using Megumin.AI.BehaviorTree;
using Megumin.Perception;
using UnityEngine;

public class CanSeeTarget : ConditionDecorator<TransformPerception>
{
    public RefVar_Transform Target;
    protected override bool OnCheckCondition(object options = null)
    {
        return Target.Value == MyAgent.AutoTarget;
    }
}

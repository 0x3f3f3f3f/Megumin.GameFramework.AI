using System.Collections;
using System.Collections.Generic;
using Megumin.Binding;
using Megumin.GameFramework.AI;
using Megumin.GameFramework.AI.BehaviorTree;
using Megumin.GameFramework.Perception;
using UnityEngine;

public class CanSeeTarget : ConditionDecorator<TransformPerception>
{
    public RefVar_Transform Target;
    protected override bool OnCheckCondition(BTNode container)
    {
        return Target.Value == MyAgent.AutoTarget;
    }
}

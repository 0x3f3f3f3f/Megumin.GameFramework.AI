using System.Collections;
using System.Collections.Generic;
using Megumin.Binding;
using Megumin.GameFramework.AI.BehaviorTree;
using UnityEngine;

public class MoveTo : BTActionNode
{
    public RefVar<Transform> Des;

    protected override void OnEnter()
    {
        base.OnEnter();
        var des = Vector3.Distance(Transform.position, Des.Value.position);
        Debug.LogError($"Distance : {des}");
    }
}

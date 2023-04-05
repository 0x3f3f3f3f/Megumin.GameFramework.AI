using System.Collections;
using System.Collections.Generic;
using Megumin.Binding;
using Megumin.GameFramework.AI.BehaviorTree;
using UnityEngine;
using UnityEngine.AI;

public class MoveTo : BTActionNode<NavMeshAgent>
{
    public RefVar<Transform> Des;

    protected override void OnEnter()
    {
        base.OnEnter();
        var des = Vector3.Distance(Transform.position, Des.Value.position);
        Debug.LogError($"Distance : {des}");
        Debug.LogError($"MyAgent : {MyAgent}");
        MyAgent.SetDestination(Des.Value.position);
    }
}

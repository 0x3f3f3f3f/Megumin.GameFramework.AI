using System.Collections;
using System.Collections.Generic;
using Megumin.Binding;
using Megumin.GameFramework.AI;
using Megumin.GameFramework.AI.BehaviorTree;
using UnityEngine;
using UnityEngine.AI;

public class MoveTo : BTActionNode<NavMeshAgent>
{
    public RefVar<Transform> Des;

    protected override void OnEnter()
    {
        Debug.LogError($"MyAgent : {MyAgent}");
        MyAgent.SetDestination(Des.Value.position);
        this.Transform.LookAt(Des.Value);
    }

    protected override Status OnTick(BTNode from)
    {
        var current = Vector3.Scale(Transform.position, new Vector3(1, 0, 1));
        var des = Vector3.Distance(current, Des.Value.position);

        Debug.Log($"Distance : {des}");
        if (des > 0.25f)
        {
            return Status.Running;
        }

        Debug.LogError($"MoveTo Succeeded: {Des.Value.name}£º{Des.Value.position}");
        return Status.Succeeded;
    }
}

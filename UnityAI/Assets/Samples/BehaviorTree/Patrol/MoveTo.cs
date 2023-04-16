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

    protected override void OnEnter(object options = null)
    {
        Debug.Log($"MoveTo OnEnter    MyAgent : {MyAgent}  Des : {Des.Value.name}");
        MyAgent.SetDestination(Des.Value.position);
        this.Transform.LookAt(Des.Value);
    }

    protected override Status OnTick(BTNode from, object options = null)
    {
        var current = Vector3.Scale(Transform.position, new Vector3(1, 0, 1));
        var des = Vector3.Distance(current, Des.Value.position);

        //Debug.Log($"Distance : {des}");
        if (des > 0.25f)
        {
            return Status.Running;
        }

        Debug.Log($"MoveTo Succeeded: {Des.Value.name} : {Des.Value.position}");
        return Status.Succeeded;
    }
}

using System.Collections;
using System.Collections.Generic;
using Megumin.Binding;
using Megumin.GameFramework.AI;
using Megumin.GameFramework.AI.BehaviorTree;
using UnityEngine;

public class GetDestination : ActionTaskNode
{
    public RefVar<List<GameObject>> DestinationList;
    public RefVar<Transform> Des;

    protected override void OnEnter()
    {
        var list = DestinationList?.Value;
        if (list == null || list.Count == 0)
        {
            System.Random random = new System.Random();
            Des.Value = list[random.Next(0, list.Count)].transform;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using Megumin.Binding;
using Megumin.GameFramework.AI;
using Megumin.GameFramework.AI.BehaviorTree;
using UnityEngine;

public class GetDestination : BTActionNode
{
    public RefVar_Transform Destination;
    public RefVar_Transform_List DestinationList;

    protected override void OnEnter()
    {
        var list = DestinationList?.Value;
        if (list == null || list.Count == 0)
        {
            Destination.value = Transform;
        }
        else
        {
            System.Random random = new System.Random();
            Destination.Value = list[random.Next(0, list.Count)].transform;
        }
    }
}

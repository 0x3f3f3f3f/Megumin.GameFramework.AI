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

    int index = 0;
    protected override void OnEnter()
    {
        var list = DestinationList?.Value;
        if (list == null || list.Count == 0)
        {
            Destination.value = Transform;
        }
        else
        {
            Destination.Value = list[index % list.Count].transform;
            index++;
        }
    }
}

public class TryGetNewDestination : ConditionDecorator
{
    public RefVar_Transform Destination;
    public RefVar_Transform_List DestinationList;

    int index = 0;
    protected override bool OnCheckCondition(BTNode container)
    {
        var list = DestinationList?.Value;
        if (list == null || list.Count == 0)
        {
            return false;
        }
        else
        {
            Destination.Value = list[index % list.Count].transform;
            index++;
            return true;
        }
    }
}



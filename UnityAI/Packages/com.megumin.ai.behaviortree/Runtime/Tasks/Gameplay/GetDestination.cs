using System.Collections;
using System.Collections.Generic;
using Megumin.Binding;
using Megumin.AI.BehaviorTree;
using UnityEngine;
using System.ComponentModel;

namespace Megumin.AI.BehaviorTree
{
    [Icon("d_networkproximitychecker icon")]
    [DisplayName("GetDestination")]
    [Category("Gameplay")]
    [AddComponentMenu("GetDestination(Transform)")]
    public class GetDestination : BTActionNode
    {
        public RefVar_Transform Destination;
        public RefVar_Transform_List DestinationList;

        int index = 0;
        protected override void OnEnter(object options = null)
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

    [Icon("d_networkproximitychecker icon")]
    [DisplayName("TryGetNewDestination")]
    [Category("Gameplay")]
    [AddComponentMenu("TryGetNewDestination(Transform)")]
    public class TryGetNewDestination : ConditionDecorator
    {
        public RefVar_Transform Destination;
        public RefVar_Transform_List DestinationList;

        int index = 0;
        protected override bool OnCheckCondition(object options = null)
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


}


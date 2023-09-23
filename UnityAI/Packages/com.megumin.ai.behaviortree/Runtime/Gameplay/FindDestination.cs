using System.Collections;
using System.Collections.Generic;
using Megumin.Binding;
using Megumin.AI.BehaviorTree;
using UnityEngine;
using System.ComponentModel;
using Megumin.Reflection;

namespace Megumin.AI.BehaviorTree
{
    [Icon("d_networkproximitychecker icon")]
    [DisplayName("FindDestination")]
    [Category("Gameplay")]
    [AddComponentMenu("FindDestination(Transform)")]
    [SerializationAlias("Megumin.AI.BehaviorTree.GetDestination")]
    [HelpURL(URL.WikiTask + "FindDestination")]
    public class FindDestination : BTActionNode
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
    [DisplayName("TryFindDestination")]
    [Category("Gameplay")]
    [AddComponentMenu("TryFindDestination(Transform)")]
    [SerializationAlias("Megumin.AI.BehaviorTree.TryGetNewDestination")]
    [SerializationAlias("Megumin.AI.BehaviorTree.TryFindDestination")]
    [HelpURL(URL.WikiDecorator + "TryFindDestination_Decorator")]
    public class TryFindDestination_Decorator : ConditionDecorator
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


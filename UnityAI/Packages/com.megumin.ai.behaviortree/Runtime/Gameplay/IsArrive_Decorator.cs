using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Megumin.Reflection;
using UnityEngine;

namespace Megumin.AI.BehaviorTree
{
    [Icon("buoyancyeffector2d icon")]
    [DisplayName("Is Arrive?")]
    [Category("Gameplay")]
    [AddComponentMenu("IsArrive(Transform)")]
    [HelpURL(URL.WikiDecorator + "IsArrive")]
    [SerializationAlias("Megumin.AI.BehaviorTree.IsArrive")]
    public class IsArrive_Decorator : ConditionDecorator
    {
        [Space]
        public Destination destination;

        [Space]
        public float StopingDistance = 0.25f;

        public bool IgnoreYAxis = true;

        protected override bool OnCheckCondition(object options = null)
        {
            return Transform.IsArrive(destination.GetDestination(), StopingDistance, IgnoreYAxis);
        }
    }

    public static class IsArriveExtension_92E68C3DEDCB402DAA5B29E31646509D
    {
        public static bool IsArrive(this Transform transform,
                                    Vector3 destination,
                                    float stopingDistance = 0.25f,
                                    bool ignoreYAxis = true)
        {
            if (transform)
            {
                var to = destination - transform.position;
                if (ignoreYAxis)
                {
                    //忽略Y轴。
                    to.y = 0;
                }

                var distance = to.magnitude;

                if (distance <= stopingDistance)
                {
                    return true;
                }
            }

            return false;
        }
    }

}

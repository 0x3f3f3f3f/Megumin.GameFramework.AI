using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Megumin.Binding;
using UnityEngine;

namespace Megumin.AI.BehaviorTree
{
    /// <summary>
    /// 比较两个物体的距离
    /// </summary>
    [Icon("d_distancejoint2d icon")]
    [DisplayName("CompareDistance")]
    [Description("CompareGameObjectDistance_Decorator")]
    public class CompareGameObjectDistance_Decorator : CompareDecorator<float>, IDetailable
    {
        [Space]
        public bool FromUseSelf = true;
        public RefVar_GameObject From;
        public RefVar_GameObject To;

        //这里不要增加SaveTo，因为只在比较时才计算一次，不是实时的。

        [Space]
        public RefVar_Float CompareTo = new RefVar_Float() { value = 0.2f };

        private float distance;

        public override float GetResult()
        {
            Vector3 fromPosition;
            if (FromUseSelf)
            {
                fromPosition = GameObject.transform.position;
            }
            else
            {
                fromPosition = From.Value.transform.position;
            }
            distance = Vector3.Distance(fromPosition, To.Value.transform.position);
            return distance;
        }

        public override float GetCompareTo()
        {
            return CompareTo;
        }

        public string GetDetail()
        {
            if (Owner.State == Status.Running)
            {
                return $"{distance} {Mode} {CompareTo.Value}";
            }

            if (CompareTo != null)
            {
                return $"{Mode} {CompareTo.Value}";
            }

            return null;
        }
    }

    /// <summary>
    /// 比较两个物体的距离
    /// </summary>
    [Icon("d_distancejoint2d icon")]
    [DisplayName("CompareDistance")]
    [Description("CompareTransformDistance_Decorator")]
    public class CompareTransformDistance_Decorator : CompareDecorator<float>, IDetailable
    {
        [Space]
        public bool FromUseSelf = true;
        public RefVar_Transform From;
        public RefVar_Transform To;

        [Space]
        public RefVar_Float CompareTo = new RefVar_Float() { value = 0.2f };

        private float distance;

        public override float GetResult()
        {
            Vector3 fromPosition;
            if (FromUseSelf)
            {
                fromPosition = GameObject.transform.position;
            }
            else
            {
                fromPosition = From.Value.transform.position;
            }
            distance = Vector3.Distance(fromPosition, To.Value.transform.position);
            return distance;
        }

        public override float GetCompareTo()
        {
            return CompareTo;
        }

        public string GetDetail()
        {
            if (Owner.State == Status.Running)
            {
                return $"{distance} {Mode} {CompareTo.Value}";
            }

            if (CompareTo != null)
            {
                return $"{Mode} {CompareTo.Value}";
            }

            return null;
        }
    }
}

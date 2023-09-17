using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Megumin.Binding;
using UnityEngine;

namespace Megumin.AI.BehaviorTree
{
    [DisplayName("CompareRandomFloat")]
    public class CompareRandomFloat_Decorator : CompareDecorator<float>
    {
        [Space]
        public bool UseRange = false;
        public RefVar_Float Min;
        public RefVar_Float Max;

        [Space]
        public RefVar_Float SaveTo;

        [Space]
        public RefVar_Float CompareTo;

        public override float GetResult()
        {
            float random = 0;
            if (UseRange)
            {
                random = Random.Range(Min, Max);
            }
            else
            {
                random = Random.value;
            }
            SaveTo?.SetValue(random);
            return random;
        }

        public override float GetCompareTo()
        {
            return CompareTo;
        }
    }
}



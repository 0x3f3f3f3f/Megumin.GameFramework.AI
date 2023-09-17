using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Megumin.Binding;
using UnityEngine;

namespace Megumin.AI.BehaviorTree
{
    [DisplayName("CompareRandomInt")]
    public class CompareRandomInt_Decorator : CompareDecorator<int>
    {
        [Space]
        public bool UseRange = false;
        public RefVar_Int Min;
        public RefVar_Int Max;

        [Space]
        public RefVar_Int SaveTo;

        [Space]
        public RefVar_Int CompareTo;

        public override int GetResult()
        {
            int random = 0;
            if (UseRange)
            {
                random = Random.Range(Min, Max);
            }
            else
            {
                random = (int)Random.value;
            }
            SaveTo?.SetValue(random);
            return random;
        }

        public override int GetCompareTo()
        {
            return CompareTo;
        }
    }
}

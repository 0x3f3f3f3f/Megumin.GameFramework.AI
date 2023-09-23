using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Megumin.Binding;
using UnityEngine;

namespace Megumin.AI.BehaviorTree
{
    [DisplayName("CompareFloat")]
    [HelpURL(URL.WikiDecorator + "CompareFloat_Decorator")]
    public class CompareFloat_Decorator : CompareDecorator<float>
    {
        public RefVar_Float Left;
        public RefVar_Float Right;

        public override float GetResult()
        {
            return Left;
        }

        public override float GetCompareTo()
        {
            return Right;
        }
    }
}

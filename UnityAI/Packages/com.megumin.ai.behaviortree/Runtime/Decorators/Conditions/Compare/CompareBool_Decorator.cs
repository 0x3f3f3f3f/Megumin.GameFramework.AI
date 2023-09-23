using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Megumin.Binding;
using UnityEngine;

namespace Megumin.AI.BehaviorTree
{
    [DisplayName("CompareBool")]
    [HelpURL(URL.WikiDecorator + "CompareBool_Decorator")]
    public class CompareBool_Decorator : CompareDecorator<bool>
    {
        public RefVar_Bool Left;
        public RefVar_Bool Right;

        public override bool GetResult()
        {
            return Left;
        }

        public override bool GetCompareTo()
        {
            return Right;
        }
    }
}

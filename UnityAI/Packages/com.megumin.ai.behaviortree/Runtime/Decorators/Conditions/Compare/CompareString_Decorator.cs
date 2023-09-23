using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Megumin.Binding;
using UnityEngine;

namespace Megumin.AI.BehaviorTree
{
    [DisplayName("CompareString")]
    [HelpURL(URL.WikiDecorator + "CompareString_Decorator")]
    public class CompareString_Decorator : CompareDecorator<string>
    {
        public RefVar_String Left;
        public RefVar_String Right;

        public override string GetResult()
        {
            return Left;
        }

        public override string GetCompareTo()
        {
            return Right;
        }
    }
}

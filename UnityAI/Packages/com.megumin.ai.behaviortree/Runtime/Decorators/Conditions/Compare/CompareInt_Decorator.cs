using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Megumin.Binding;
using UnityEngine;

namespace Megumin.AI.BehaviorTree
{
    [DisplayName("CompareInt")]
    [HelpURL(URL.WikiDecorator + "CompareInt_Decorator")]
    public class CompareInt_Decorator : CompareDecorator<int>
    {
        public RefVar_Int Left;
        public RefVar_Int Right;

        public override int GetResult()
        {
            return Left;
        }

        public override int GetCompareTo()
        {
            return Right;
        }
    }
}

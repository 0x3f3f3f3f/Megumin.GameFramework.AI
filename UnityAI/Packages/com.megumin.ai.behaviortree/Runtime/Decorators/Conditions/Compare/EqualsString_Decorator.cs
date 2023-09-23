using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Megumin.Binding;
using Megumin.Reflection;
using UnityEngine;

namespace Megumin.AI.BehaviorTree
{
    [DisplayName("EqualsString")]
    [SerializationAlias("Megumin.AI.BehaviorTree.CheckStringEquals")]
    [HelpURL(URL.WikiDecorator + "EqualsString_Decorator")]
    public class EqualsString_Decorator : StringEqualsDecorator
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




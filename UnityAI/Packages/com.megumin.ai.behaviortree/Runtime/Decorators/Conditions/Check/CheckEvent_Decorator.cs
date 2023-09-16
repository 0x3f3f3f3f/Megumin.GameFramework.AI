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
    [HelpURL(URL.WikiDecorator + "CheckEvent")]
    [DisplayName("CheckEvent")]
    [SerializationAlias("Megumin.AI.BehaviorTree.CheckEvent")]
    public class CheckEvent_Decorator : ConditionDecorator, IDetailable
    {
        public RefVar_String EventName;

        protected override bool OnCheckCondition(object options = null)
        {
            if (Tree.TryGetEvent(EventName, Owner, out var eventData))
            {
                return true;
            }
            return false;
        }

        public string GetDetail()
        {
            return @$"Name: ""{(string)EventName}""";
        }
    }
}


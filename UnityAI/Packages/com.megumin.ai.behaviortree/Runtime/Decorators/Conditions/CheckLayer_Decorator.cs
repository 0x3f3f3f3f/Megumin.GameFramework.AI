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
    [HelpURL(URL.WikiDecorator + "CheckLayer")]
    [DisplayName("CheckLayer")]
    [SerializationAlias("Megumin.AI.BehaviorTree.CheckLayer")]
    public sealed class CheckLayer_Decorator : ConditionDecorator<GameObject>
    {
        [Space]
        [Tooltip("true Check Self. false Check Target")]
        public bool CheckSelfOrTarget = false;

        public RefVar_GameObject Target;
        public LayerMask LayerMask = -1;

        protected override bool OnCheckCondition(object options = null)
        {
            if (CheckSelfOrTarget)
            {
                if (!MyAgent)
                {
                    return false;
                }

                if ((1 << MyAgent.layer & LayerMask) != 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                if (Target == null)
                {
                    return false;
                }

                if ((1 << Target.Value.layer & LayerMask) != 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

        }
    }
}




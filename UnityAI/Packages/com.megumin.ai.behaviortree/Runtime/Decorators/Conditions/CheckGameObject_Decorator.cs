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

    [HelpURL(URL.WikiDecorator + "CheckGameObject")]
    [DisplayName("CheckGameObject")]
    [SerializationAlias("Megumin.AI.BehaviorTree.CheckGameObject")]
    public sealed class CheckGameObject_Decorator : ConditionDecorator<GameObject>
    {
        [Space]
        [Tooltip("true Check Self. false Check Target")]
        public bool CheckSelfOrTarget = false;

        public RefVar_GameObject Target;
        public GameObjectFilter Filter;
        protected override bool OnCheckCondition(object options = null)
        {
            if (CheckSelfOrTarget)
            {
                return Filter?.Check(MyAgent) ?? true;
            }
            else
            {
                return Filter?.Check(Target) ?? true;
            }
        }
    }
}



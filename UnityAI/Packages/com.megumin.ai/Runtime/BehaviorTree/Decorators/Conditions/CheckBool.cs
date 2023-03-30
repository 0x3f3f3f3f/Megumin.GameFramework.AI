using System;
using Megumin.Reflection;
using UnityEngine.Serialization;

namespace Megumin.GameFramework.AI.BehaviorTree
{
    //[FormerlySerializedAs("CheckBool")]
    [Serializable]
    [SerializationAlias("CheckBool")]
    public class CheckBool : ConditionDecorator, IConditionDecorator
    {
        [FormerlySerializedAs("A")]
        public bool Success = false;

        protected override bool OnCheckCondition(BTNode container)
        {
            return Success;
        }
    }
}


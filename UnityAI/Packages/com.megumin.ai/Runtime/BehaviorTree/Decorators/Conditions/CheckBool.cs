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
        public bool A = false;
        [FormerlySerializedAs("B")]
        public bool C = false;
        public bool Cal()
        {
            return A;
        }

        public bool Result { get; set; }
    }
}


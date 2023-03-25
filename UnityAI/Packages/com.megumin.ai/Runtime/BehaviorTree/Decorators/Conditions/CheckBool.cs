using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Serialization;

namespace Megumin.GameFramework.AI.BehaviorTree
{
    //[FormerlySerializedAs("CheckBool")]
    [Serializable]
    public class CheckBool : ConditionDecorator, IConditionDecorator
    {
        [FormerlySerializedAs("A")]
        public bool A = false;
        [FormerlySerializedAs("B")]
        public bool B = false;
        public bool Cal()
        {
            return A;
        }

        public bool Result { get; set; }
    }
}



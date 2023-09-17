using System.Collections;
using System.Collections.Generic;
using Megumin.Binding;
using UnityEngine;

namespace Megumin.AI.BehaviorTree
{
    public sealed class RandomFloat : BTActionNode
    {
        [Space]
        public bool UseRange = false;
        public RefVar_Float Min;
        public RefVar_Float Max;

        [Space]
        public RefVar_Float SaveTo;

        protected override void OnEnter(object options = null)
        {
            base.OnEnter(options);
            if (UseRange)
            {
                SaveTo?.SetValue(Random.Range(Min, Max));
            }
            else
            {
                SaveTo?.SetValue(Random.value);
            }
        }
    }
}



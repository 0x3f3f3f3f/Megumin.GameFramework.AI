using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Megumin.Binding;
using UnityEngine;

namespace Megumin.AI.BehaviorTree
{
    public sealed class RandomInt : BTActionNode
    {
        [Space]
        public bool UseRange = false;
        public RefVar_Int Min;
        public RefVar_Int Max;

        [Space]
        public RefVar_Int SaveTo;

        protected override void OnEnter(object options = null)
        {
            base.OnEnter(options);
            if (UseRange)
            {
                SaveTo?.SetValue(Random.Range(Min, Max));
            }
            else
            {
                SaveTo?.SetValue(((int)Random.value));
            }
        }
    }
}


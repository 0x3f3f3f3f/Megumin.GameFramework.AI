using System.Collections;
using System.Collections.Generic;
using Megumin.Binding;
using UnityEngine;

namespace Megumin.AI.BehaviorTree
{
    /// <summary>
    /// 随机浮点
    /// 随机一个浮点数，并保存到SaveTo中。
    /// </summary>
    [HelpURL(URL.WikiTask + "RandomFloat")]
    public sealed class RandomFloat : BTActionNode
    {
        [Space]
        public bool UseRange = false;
        public RefVar_Float Min;
        public RefVar_Float Max;

        [Space]
        public RefVar_Float SaveTo;

        protected override void OnEnter(BTNode from, object options = null)
        {
            base.OnEnter(from, options);
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



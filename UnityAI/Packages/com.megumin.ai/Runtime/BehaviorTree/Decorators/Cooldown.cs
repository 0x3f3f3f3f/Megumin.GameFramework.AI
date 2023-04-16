using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Megumin.Binding;
using UnityEngine;

namespace Megumin.GameFramework.AI.BehaviorTree
{
    public enum CooldownMode
    {
        OnEnter,
        OnSucceeded,
        OnFailed,
    }

    /// <summary>
    /// CD
    /// </summary>
    public class Cooldown : ConditionDecorator, IPreDecorator, IPostDecorator, IConditionDecorator
    {
        public RefVar_Double CooldownTime = new RefVar_Double() { value = 5 };
        public CooldownMode Mode = CooldownMode.OnEnter;

        protected double nextCanEnterTime = -1;

        protected override bool OnCheckCondition(object options = null)
        {
            return Time.time > nextCanEnterTime;
        }

        public Status AfterNodeExit(Status result, object options = null)
        {
            if (Mode == CooldownMode.OnSucceeded && result == Status.Succeeded)
            {
                nextCanEnterTime = Time.time + CooldownTime;
            }
            else if (Mode == CooldownMode.OnFailed && result == Status.Failed)
            {
                nextCanEnterTime = Time.time + CooldownTime;
            }

            return result;
        }

        public void BeforeNodeEnter(object options = null)
        {
            if (Mode == CooldownMode.OnEnter)
            {
                nextCanEnterTime = Time.time + CooldownTime;
            }
        }
    }
}

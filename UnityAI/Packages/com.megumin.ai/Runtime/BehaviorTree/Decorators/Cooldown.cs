using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Megumin.GameFramework.AI.BehaviorTree
{
    /// <summary>
    /// 成功执行后进入CD.
    /// </summary>
    public class Cooldown : ConditionDecorator, IPostDecorator, IConditionDecorator
    {
        public double cooldownTime = 5;
        public double nextCanEnterTime;
        public Status AfterNodeExit(Status result, BTNode bTNode)
        {
            if (result == Status.Succeeded)
            {
                nextCanEnterTime = Time.time + cooldownTime;
            }

            return result;
        }

        protected override bool OnCheckCondition()
        {
            return Time.time > nextCanEnterTime;
        }
    }
}

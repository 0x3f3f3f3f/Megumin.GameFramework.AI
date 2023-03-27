using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Megumin.GameFramework.AI.BehaviorTree
{
    /// <summary>
    /// 
    /// </summary>
    public class Repeater : OneChildNode
    {
        public int loopCount = 2;

        int cur = 0;
        protected override Status OnTick(BTNode from)
        {
            if (loopCount == 0)
            {
                return base.OnTick(from);
            }

            var res = Child0.Tick(this);

            if (res == Status.Succeeded || res == Status.Failed)
            {
                cur++;
                Log($"Repeater: complete {cur}");
                if (loopCount >= 0 && cur >= loopCount)
                {
                    return res;
                }
            }

            return Status.Running;
        }
    }
}

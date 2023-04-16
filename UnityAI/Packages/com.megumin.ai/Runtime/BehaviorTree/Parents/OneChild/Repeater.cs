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
    public class Repeater : OneChildNode, IDetailable
    {
        public int loopCount = 2;

        int completeCount = 0;

        protected override void OnEnter(object options = null)
        {
            completeCount = 0;
        }

        protected override Status OnTick(BTNode from, object options = null)
        {
            if (loopCount == 0)
            {
                return base.OnTick(from);
            }

            var res = Child0.Tick(this);

            if (res == Status.Succeeded || res == Status.Failed)
            {
                completeCount++;
                Log($"Repeater: complete {completeCount}");
                if (loopCount >= 0 && completeCount >= loopCount)
                {
                    return res;
                }
            }

            return Status.Running;
        }

        public string GetDetail()
        {
            return $"Count: {completeCount} / {loopCount}";
        }
    }
}

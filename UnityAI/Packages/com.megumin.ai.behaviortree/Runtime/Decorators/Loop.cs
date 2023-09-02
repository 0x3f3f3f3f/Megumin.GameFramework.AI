using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Megumin.AI.BehaviorTree
{
    public class Loop : BTDecorator, IPostDecorator, IAbortDecorator, IDetailable
    {
        /// <summary>
        /// 作为独立AI使用，根节点可能设置为无限循环。
        /// 如果作为其他树的子树，可能会忽略循环，改为单次执行。
        /// </summary>
        public bool IgnoreInSubTree = false;

        [Tooltip("-1: infinite loop. 0: complete once")]
        public int loopCount = -1;

        int completeCount = 0;
        public Status AfterNodeExit(Status result, object options = null)
        {
            if (IgnoreInSubTree && Tree.IsSubtree)
            {
                return result;
            }

            completeCount++;
            GetLogger()?.WriteLine($"loop: complete {completeCount}.    loopCount:{loopCount}");
            if (completeCount >= loopCount && loopCount >= 0)
            {
                completeCount = 0;
                return result;
            }
            return Status.Running;
        }

        public void OnNodeAbort(object options = null)
        {
            completeCount = 0;
        }

        public string GetDetail()
        {
            return $"Count: {completeCount} / {loopCount}";
        }
    }
}



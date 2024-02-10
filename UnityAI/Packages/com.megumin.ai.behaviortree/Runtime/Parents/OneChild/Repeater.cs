using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Megumin.AI.BehaviorTree
{
    /// <summary>
    /// 重复
    /// </summary>
    [HelpURL(URL.WikiTask + "Repeater")]
    public class Repeater : OneChildNode, IDetailable
    {
        /// <summary>
        /// 作为独立AI使用，根节点可能设置为无限循环。
        /// 如果作为其他树的子树，可能会忽略循环，改为单次执行。
        /// </summary>
        public bool IgnoreInSubTree = false;

        [Tooltip("-1: infinite loop. 0: complete once")]
        public int loopCount = -1;

        int completeCount = 0;

        protected override void OnEnter(BTNode from, object options = null)
        {
            completeCount = 0;
        }

        protected override Status OnTick(BTNode from, object options = null)
        {
            var result = Child0.Tick(this);

            if (IgnoreInSubTree && Tree.IsSubtree)
            {
                return result;
            }

            if (result == Status.Succeeded || result == Status.Failed)
            {
                completeCount++;
                GetLogger()?.WriteLine($"Repeater: complete {completeCount}");
                if (completeCount >= loopCount && loopCount >= 0)
                {
                    completeCount = 0;
                    return result;
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

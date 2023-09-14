using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Megumin.Binding;
using Megumin.Timers;
using UnityEngine;

namespace Megumin.AI.BehaviorTree
{
    /// <summary>
    /// 超时节点
    /// </summary>
    /// <remarks>
    /// 如果用装饰器实现，需要条件装饰标记为AbortSelf。
    /// </remarks>
    public class Timeout : TimerParent
    {
        protected override Status OnTick(BTNode from, object options = null)
        {
            if (WaitTimeable.WaitEnd(WaitTime))
            {
                Child0.Abort(this);
                return Status.Failed;
            }

            return Child0?.Tick(this, options) ?? GetIgnoreResult(from);
        }
    }
}






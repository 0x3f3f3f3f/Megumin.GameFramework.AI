using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Megumin.Binding;
using UnityEngine;

namespace Megumin.GameFramework.AI.BehaviorTree
{
    /// <summary>
    /// 超时节点
    /// </summary>
    /// <remarks>
    /// 如果用装饰器实现，需要条件装饰标记为AbortSelf。
    /// </remarks>
    public class Timeout : OneChildNode
    {
        public RefVar_Float Duration = new() { value = 30f };
        float startTime;

        protected override void OnEnter(object options = null)
        {
            startTime = Time.time;
        }

        protected override Status OnTick(BTNode from, object options = null)
        {
            if (Time.time - startTime > Duration)
            {
                Child0.Abort(this);
                return Status.Failed;
            }
            return base.OnTick(from);
        }
    }
}






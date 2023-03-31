using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        public float duration = 1.0f;
        float startTime;

        protected override void OnEnter()
        {
            startTime = Time.time;
        }

        protected override Status OnTick(BTNode from)
        {
            if (Time.time - startTime > duration)
            {
                Child0.Abort(this);
                return Status.Failed;
            }
            return base.OnTick(from);
        }
    }
}






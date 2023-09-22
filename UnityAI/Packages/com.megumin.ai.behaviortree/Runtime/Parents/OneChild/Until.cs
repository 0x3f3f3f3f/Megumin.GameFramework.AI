using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Megumin.AI.BehaviorTree
{
    /// <summary>
    /// 直到节点
    /// 无限循环执行子节点，直到子节点满足设定的结果。
    /// </summary>
    [HelpURL(URL.WikiTask + "Until")]
    public class Until : OneChildNode, IDetailable
    {
        public Status Result = Status.Succeeded;
        protected override Status OnTick(BTNode from, object options = null)
        {
            if (Child0 == null)
            {
                return GetIgnoreResult(from);
            }

            var res = Child0.Tick(this);

            if (res == Result)
            {
                return res;
            }

            return Status.Running;
        }

        public string GetDetail()
        {
            return Result.ToString();
        }
    }
}

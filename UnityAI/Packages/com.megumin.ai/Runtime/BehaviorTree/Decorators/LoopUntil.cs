using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Megumin.GameFramework.AI.BehaviorTree
{
    /// <summary>
    /// 持续循环知道返回想要的结果。
    /// </summary>
    internal class LoopUntil : BTDecorator, IPostDecorator
    {
        public Status Result = Status.Succeeded;
        public Status AfterNodeExit(Status result, BTNode bTNode)
        {
            if (result == Result)
            {
                return result;
            }
            return Status.Running;
        }
    }
}

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
    public class LoopUntil : BTDecorator, IPostDecorator
    {
        public Status Result = Status.Succeeded;
        public Status AfterNodeExit(Status result, object options = null)
        {
            if (result == Result)
            {
                return result;
            }
            return Status.Running;
        }
    }
}

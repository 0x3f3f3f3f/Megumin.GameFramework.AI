using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Megumin.GameFramework.AI.BehaviorTree
{
    public class Inverter : BTDecorator, IPostDecorator
    {
        public Status AfterNodeExit(Status result, object options = null)
        {
            //可能会出现结果是Running的情况，比如有LoopUntil在此之前执行，将结果改成了Running。
            //所以只处理成功和失败两种。
            if (result == Status.Failed)
            {
                return Status.Succeeded;
            }
            else if (result == Status.Succeeded)
            {
                return Status.Failed;
            }

            return result;
        }
    }
}

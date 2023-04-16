using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Megumin.GameFramework.AI.BehaviorTree
{
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

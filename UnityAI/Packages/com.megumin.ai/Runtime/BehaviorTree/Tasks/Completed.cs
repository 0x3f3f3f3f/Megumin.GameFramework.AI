using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Megumin.GameFramework.AI.BehaviorTree
{
    [Category("Action")]
    public sealed class Completed : BTActionNode
    {
        public bool Success = true;
        protected override Status OnTick(BTNode from, object options = null)
        {
            return Success ? Status.Succeeded : Status.Failed;
        }
    }
}

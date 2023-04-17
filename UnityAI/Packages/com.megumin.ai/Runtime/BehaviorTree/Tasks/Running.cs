using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Megumin.GameFramework.AI.BehaviorTree
{
    [Category("Action")]
    public class Running : BTActionNode
    {
        protected override Status OnTick(BTNode from, object options = null)
        {
            return Status.Running;
        }
    }
}



using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Megumin.Binding;

namespace Megumin.GameFramework.AI.BehaviorTree
{
    public class SetTrigger : BTActionNode, IDetailable
    {
        public RefVar<string> TriggerName;
        protected override Status OnTick(BTNode from)
        {
            Tree.SetTrigger(TriggerName, this);
            return Status.Succeeded;
        }

        public string GetDetail()
        {
            return @$"Set ""{TriggerName?.Value}""";
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Megumin.Binding;

namespace Megumin.GameFramework.AI.BehaviorTree
{
    [Category("Action")]
    public class SetTrigger : BTActionNode, IDetailable
    {
        public RefVar_String TriggerName;
        protected override Status OnTick(BTNode from, object options = null)
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

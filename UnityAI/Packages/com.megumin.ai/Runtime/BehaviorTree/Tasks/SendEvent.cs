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
    public class SendEvent : BTActionNode, IDetailable
    {
        public RefVar_String EventName;
        protected override Status OnTick(BTNode from, object options = null)
        {
            Tree.SendEvent(EventName, this);
            return Status.Succeeded;
        }

        public string GetDetail()
        {
            return @$"Send ""{EventName?.Value}"".";
        }
    }
}

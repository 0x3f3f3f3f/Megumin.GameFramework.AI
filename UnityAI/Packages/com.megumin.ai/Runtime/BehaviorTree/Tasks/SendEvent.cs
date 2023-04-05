using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Megumin.Binding;

namespace Megumin.GameFramework.AI.BehaviorTree
{
    public class SendEvent : BTActionNode, IDetailable
    {
        public RefVar<string> EventName;
        protected override Status OnTick(BTNode from)
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

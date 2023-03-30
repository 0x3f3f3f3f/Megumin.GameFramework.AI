using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Megumin.Binding;

namespace Megumin.GameFramework.AI.BehaviorTree
{
    public class SendEvent : ActionTaskNode
    {
        public RefVar<string> EventName;
        protected override Status OnTick(BTNode from)
        {
            Tree.SendEvent(EventName,this);
            return base.OnTick(from);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace Megumin.GameFramework.AI.BehaviorTree
{
    public class Controller
    {
        public BehaviorTree BehaviorTree { get; set; }
        public object Controlled { get; set; }
    }
}

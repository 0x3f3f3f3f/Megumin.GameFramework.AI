using System;
using System.Collections.Generic;
using System.Text;

namespace Megumin.GameFramework.AI.BehaviorTree
{
    public class BehaviorTree
    {
        public object Agent { get; set; }
        public static object GlobalAgent { get; set; }
        public Controller Controller { get; set; }
    }
}

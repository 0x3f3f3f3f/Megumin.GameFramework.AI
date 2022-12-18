using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Megumin.GameFramework.AI.BehaviorTree
{
    public class Wait : TaskNode
    {
        float entertime;
        public override void OnEnter()
        {
            entertime = Time.time;
        }

        public override TaskNodeTickResult OnTick()
        {
            if (Time.time - entertime > 5)
            {
                return TaskNodeTickResult.Succeeded;
            }
            return TaskNodeTickResult.Running;
        }
    }
}

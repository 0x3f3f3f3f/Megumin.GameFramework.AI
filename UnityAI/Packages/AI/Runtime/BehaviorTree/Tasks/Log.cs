using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Megumin.GameFramework.AI.BehaviorTree
{
    public class Log: TaskNode
    {
        int count = 0;

        public override void OnEnter()
        {
            count++;
        }

        public override TaskNodeTickResult OnTick()
        {
            Debug.Log($"Hello world! {count}");
            return base.OnTick();
        }
    }
}

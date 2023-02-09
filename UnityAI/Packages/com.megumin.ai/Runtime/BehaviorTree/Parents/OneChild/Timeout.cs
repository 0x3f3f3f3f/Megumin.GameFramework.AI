using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Megumin.GameFramework.AI.BehaviorTree
{
    public class Timeout : OneChildNode
    {
        public float duration = 1.0f;
        float startTime;

        protected override void OnEnter()
        {
            startTime = Time.time;
        }

        protected override Status OnTick()
        {
            if (Time.time - startTime > duration)
            {
                Child0.Abort();
                return Status.Failed;
            }
            return base.OnTick();
        }
    }
}






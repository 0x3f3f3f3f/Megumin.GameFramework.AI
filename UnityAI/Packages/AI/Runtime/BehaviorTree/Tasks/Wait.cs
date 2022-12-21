using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Megumin.GameFramework.AI.BehaviorTree
{
    public class Wait : ActionTaskNode
    {
        float entertime;
        protected override void OnEnter()
        {
            entertime = Time.time;
        }

        protected override Status OnTick()
        {
            Debug.Log($"Wait Time :{Time.time - entertime}");
            if (Time.time - entertime > 2)
            {
                return Status.Succeeded;
            }
            return Status.Running;
        }
    }
}

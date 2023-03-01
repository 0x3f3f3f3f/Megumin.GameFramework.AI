using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Megumin.GameFramework.AI.BehaviorTree
{
    [Category("CategoryTest")]
    public class Wait : ActionTaskNode
    {
        public float waitTime = 1f;

        float entertime;
        protected override void OnEnter()
        {
            entertime = Time.time;
        }

        protected override Status OnTick()
        {
            //Debug.Log($"Wait Time :{Time.time - entertime}");
            if (Time.time - entertime > waitTime)
            {
                return Status.Succeeded;
            }
            return Status.Running;
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Megumin.GameFramework.AI.BehaviorTree
{
    public class Loop : BTDecorator, IPostDecorator, IAbortDecorator, IDetailable
    {
        public int loopCount = -1;

        int completeCount = 0;
        public Status AfterNodeExit(Status result, object options = null)
        {
            completeCount++;
            Log($"loop: complete {completeCount}.    loopCount:{loopCount}");
            if (completeCount >= loopCount && loopCount > 0)
            {
                completeCount = 0;
                return result;
            }
            return Status.Running;
        }

        public void OnNodeAbort(object options = null)
        {
            completeCount = 0;
        }

        public string GetDetail()
        {
            return $"Count: {completeCount} / {loopCount}";
        }
    }
}



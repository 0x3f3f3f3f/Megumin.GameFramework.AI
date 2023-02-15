using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Megumin.GameFramework.AI.BehaviorTree
{
    public class Loop : BTDecorator, IPostDecirator, IAbortDecirator
    {
        public int loopCount = 2;

        int cur = 0;
        public Status AfterNodeExit(Status result, BTNode bTNode)
        {
            cur++;
            Debug.Log($"loop: complete {cur}");
            if (cur >= loopCount)
            {
                cur = 0;
                return result;
            }
            return Status.Running;
        }

        public void OnNodeAbort(BTNode bTNode)
        {
            cur = 0;
        }
    }
}



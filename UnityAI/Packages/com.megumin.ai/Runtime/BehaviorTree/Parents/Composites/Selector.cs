using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Megumin.GameFramework.AI.BehaviorTree
{
    public class Selector : CompositeNode
    {
        protected override Status OnTick(BTNode from)
        {
            for (int i = 0; i < children.Count; i++)
            {
                BTNode target = null;

                var child = children[i];
                if (i >= current)
                {
                    target = child;
                }
                else
                {
                    if (Dynamic || child.HasAbortLowerPriorityFlag())
                    {
                        target = child;
                    }
                }

                void TryAbortLastRunning()
                {
                    if (i < current)
                    {
                        //终止成功
                        var lastRunning = children[current];
                        Log($"{child} AbortLowerPriority {lastRunning}");
                        lastRunning.Abort(this);
                    }
                }

                if (target != null)
                {
                    var result = target.Tick(this);
                    if (result == Status.Running)
                    {
                        TryAbortLastRunning();
                        current = i;
                        return Status.Running;
                    }
                    else if (result == Status.Succeeded)
                    {
                        TryAbortLastRunning();
                        current = i;
                        return Status.Succeeded;
                    }

                    //指针只能向右移动
                    current = Math.Max(current, i);
                }
            }

            return Status.Failed;
        }
    }
}

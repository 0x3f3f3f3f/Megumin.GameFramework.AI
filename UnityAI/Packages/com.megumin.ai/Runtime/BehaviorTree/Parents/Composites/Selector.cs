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
        protected override Status OnTick()
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
                    if (dynamicAbort || child.CanAbortLowerPriority())
                    {
                        //一种情况是，当条件装饰一直成功，但是节点本身一直失败，
                        //这时不能终止正在运行的节点，否则会导致不停的终止重新进入。
                        //所以触发终止应该放在节点本身tick之后，如果没有失败，则触发终止。
                        if (child.CanExecute())
                        {
                            target = child;
                            //这里标记一下已经检查过了，马上下一次Tick就不要重复检查，防止连续调用2次。
                            target.IsCheckedCanExecute = true;
                        }
                    }
                }

                void AbortLastRunning()
                {
                    if (current != i)
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
                        AbortLastRunning();
                        current = i;
                        return Status.Running;
                    }
                    else if (result == Status.Succeeded)
                    {
                        AbortLastRunning();
                        current = i;
                        return Status.Succeeded;
                    }

                    current = i;
                }
            }

            return Status.Failed;
        }
    }
}

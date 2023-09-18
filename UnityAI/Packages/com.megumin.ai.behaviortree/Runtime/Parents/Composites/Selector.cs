using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Megumin.AI.BehaviorTree
{
    /// <summary>
    /// 选择节点 <![CDATA[||]]>
    /// <para/> 节点按从左到右的顺序执行其子节点。当其中一个子节点执行成功时，选择器节点将停止执行。如果选择器的一个子节点成功运行，则选择器运行成功。如果选择器的所有子节点运行失败，则选择器运行失败。
    /// </summary>
    public class Selector : CompositeNode, IDynamicable
    {
        protected override Status OnTick(BTNode from, object options = null)
        {
            for (int i = 0; i < Children.Count; i++)
            {
                BTNode target = null;

                var child = Children[i];
                if (i >= CurrentIndex)
                {
                    target = child;
                }
                else
                {
                    if (Dynamic || child.HasAbortLowerPriorityFlag())
                    {
                        //仅当前面的节点可运行或者可成功，才进行低优先级终止
                        //如果前面的节点不包含低优先级终止，就会跳过
                        //如果前面的节点包含低优先级终止，但是节点本身执行失败，也不会打断后面的节点

                        //合并上面的情况，优化代码后，就是
                        //如果含有低优先级终止标记的装饰器，就每次执行前面的节点，
                        //一旦前面的节点可以运行或者成功，就终止掉后面的节点
                        target = child;
                    }
                }

                void TryAbortLastRunning()
                {
                    if (i < CurrentIndex)
                    {
                        //终止成功
                        var lastRunning = Children[CurrentIndex];
                        GetLogger()?.WriteLine($"{child} AbortLowerPriority {lastRunning}");
                        lastRunning.Abort(this, options);
                    }
                }

                if (target != null)
                {
                    var result = target.Tick(this, options);
                    if (result == Status.Running)
                    {
                        TryAbortLastRunning();
                        CurrentIndex = i;
                        return Status.Running;
                    }
                    else if (result == Status.Succeeded)
                    {
                        TryAbortLastRunning();
                        CurrentIndex = i;
                        return Status.Succeeded;
                    }

                    //指针只能向右移动
                    CurrentIndex = Math.Max(CurrentIndex, i);
                }
            }

            return Status.Failed;
        }
    }
}

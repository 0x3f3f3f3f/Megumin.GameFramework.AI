using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Megumin.AI.BehaviorTree
{
    /// <summary>
    /// 加权随机新的执行顺序，不支持低优先级终止
    /// </summary>
    [HelpURL(URL.WikiTask + "RandomSequence")]
    public class RandomSequence : RandomComposite
    {
        protected override Status OnTick(BTNode from, object options = null)
        {
            //根据Order执行子节点
            for (int i = CurrentIndex; i < CurrentOrder.Count; i++)
            {
                var index = CurrentOrder[i];
                if (Children.Count > index)
                {
                    var child = Children[index];
                    if (child == null)
                    {
                        continue;
                    }
                    else
                    {
                        var result = child.Tick(from, options);
                        if (result == Status.Running)
                        {
                            CurrentIndex = i;
                            return Status.Running;
                        }
                        else if (result == Status.Failed)
                        {
                            CurrentIndex = i;
                            return Status.Failed;
                        }
                    }
                }

                //指针只能向右移动
                CurrentIndex = Math.Max(CurrentIndex, i);
            }

            return Status.Succeeded;
        }
    }
}




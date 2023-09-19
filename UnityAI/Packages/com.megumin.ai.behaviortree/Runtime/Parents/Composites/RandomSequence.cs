using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Megumin.AI.BehaviorTree
{
    /// <summary>
    /// ��Ȩ����µ�ִ��˳�򣬲�֧�ֵ����ȼ���ֹ
    /// </summary>
    public class RandomSequence : RandomComposite
    {
        protected override Status OnTick(BTNode from, object options = null)
        {
            //����Orderִ���ӽڵ�
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

                //ָ��ֻ�������ƶ�
                CurrentIndex = Math.Max(CurrentIndex, i);
            }

            return Status.Succeeded;
        }
    }
}




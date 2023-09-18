using System.Collections;
using System.Collections.Generic;
using Megumin.Binding;
using UnityEngine;

namespace Megumin.AI.BehaviorTree
{
    /// <summary>
    /// 加权随机选出一个子节点执行
    /// </summary>
    public class RandomOne : CompositeNode
    {
        public List<int> priority;
        BTNode currentChild;
        protected override void OnEnter(object options = null)
        {
            base.OnEnter(options);
            currentChild = null;

            if (Children == null || Children.Count == 0)
            {

            }
            else
            {
                int random = Random.Range(100, 1000);
                CurrentIndex = random % Children.Count;
                currentChild = Children[CurrentIndex];
            }
        }

        protected override Status OnTick(BTNode from, object options = null)
        {
            if (currentChild != null)
            {
                return currentChild.Tick(from, options);
            }
            else
            {
                return Status.Failed;
            }
        }
    }
}




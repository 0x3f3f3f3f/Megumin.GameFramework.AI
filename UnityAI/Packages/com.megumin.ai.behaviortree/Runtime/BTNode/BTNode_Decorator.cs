using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Megumin.AI.BehaviorTree
{

    public partial class BTNode
    {
        /// <summary>
        /// 检查所有条件装饰器，能否继续执行
        /// </summary>
        /// <param name="options"></param>
        /// <returns>
        /// </returns>
        protected bool ExecuteConditionDecorator(object options = null)
        {
            foreach (var pre in Decorators)
            {
                if (pre is IConditionDecorator conditionable)
                {
                    if (conditionable.CheckCondition(options) == false)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// 检查所有含有终止自身标记的条件装饰器
        /// </summary>
        /// <returns>
        /// <see langword="true"/>结果值全为true
        /// <see langword="false"/>结果值含有false
        /// </returns>
        protected bool ExecuteSelfAbortable(object options = null)
        {
            foreach (var deco in Decorators)
            {
                if (deco is IAbortable abortable && (abortable.AbortType & AbortType.Self) != 0)
                {
                    if (deco is IConditionDecorator conditionable && conditionable.CheckCondition(options) == false)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// 检查所有含有终止低优先级标记的条件装饰器
        /// </summary>
        /// <returns>
        /// <see langword="true"/>结果值全为true
        /// <see langword="false"/>结果值含有false
        /// </returns>
        protected bool ExecuteLowerPriorityAbortable(object options = null)
        {
            foreach (var deco in Decorators)
            {
                if (deco is IAbortable abortable && (abortable.AbortType & AbortType.LowerPriority) != 0)
                {
                    if (deco is IConditionDecorator conditionable && conditionable.CheckCondition(options) == false)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// 调用前置装饰器
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        private Status ExecutePreDecorator(object options = null)
        {
            var res = Status.Running;

            foreach (var pre in Decorators)
            {
                if (pre is IPreDecorator decirator)
                {
                    decirator.BeforeNodeEnter(options);
                }
            }

            return res;
        }

        private Status ExecutePostDecorator(object options = null)
        {
            var res = State;
            //倒序遍历
            for (int i = Decorators.Count - 1; i >= 0; i--)
            {
                var post = Decorators[i];
                if (post is IPostDecorator decirator)
                {
                    res = decirator.AfterNodeExit(res);
                }
            }

            return res;
        }

        private Status ExecuteAbortDecorator(object options = null)
        {
            //倒序遍历
            for (int i = Decorators.Count - 1; i >= 0; i--)
            {
                var pre = Decorators[i];
                if (pre is IAbortDecorator decirator)
                {
                    decirator.OnNodeAbort(options);
                }
            }

            return State;
        }
    }
}




using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Megumin.GameFramework.AI.BehaviorTree
{

    public partial class BTNode
    {
        private bool ExecuteConditionDecorator()
        {
            foreach (var pre in Decorators)
            {
                if (pre is IConditionDecorator conditionable)
                {
                    if (conditionable.CheckCondition() == false)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// 检查含有终止自身标记的条件装饰器，能否继续执行
        /// </summary>
        /// <returns>
        /// <see langword="true"/>可以继续执行
        /// <see langword="false"/>不能继续执行，应该终止自身
        /// </returns>
        protected bool ExecuteConditionDecoratorCheckAbortSelf()
        {
            foreach (var pre in Decorators)
            {
                if (pre is IConditionDecorator conditionable)
                {
                    if ((conditionable.AbortType & AbortType.Self) != 0)
                    {
                        if (conditionable.CheckCondition() == false)
                        {
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// 检查含有终止低优先级标记的条件装饰器，能否继续执行
        /// </summary>
        /// <returns></returns>
        protected bool ExecuteConditionDecoratorCheckAbortLowerPriority()
        {
            foreach (var pre in Decorators)
            {
                if (pre is IConditionDecorator conditionable)
                {
                    if ((conditionable.AbortType & AbortType.LowerPriority) != 0)
                    {
                        if (conditionable.CheckCondition() == false)
                        {
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// 调用前置装饰器
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        private Status ExecutePreDecorator()
        {
            var res = Status.Running;

            foreach (var pre in Decorators)
            {
                if (pre is IPreDecorator decirator)
                {
                    decirator.BeforeNodeEnter();
                }
            }

            return res;
        }

        private Status ExecutePostDecorator()
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

        private Status ExecuteAbortDecorator()
        {
            //倒序遍历
            for (int i = Decorators.Count - 1; i >= 0; i--)
            {
                var pre = Decorators[i];
                if (pre is IAbortDecorator decirator)
                {
                    decirator.OnNodeAbort();
                }
            }

            return State;
        }
    }
}




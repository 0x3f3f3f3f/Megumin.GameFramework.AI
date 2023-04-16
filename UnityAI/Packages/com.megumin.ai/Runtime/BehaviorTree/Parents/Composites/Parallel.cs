using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Megumin.GameFramework.AI.BehaviorTree
{
    public enum FinishMode
    {
        /// <summary>
        /// 任意一个子节点失败，返回失败。
        /// </summary>
        AnyFailed = 0,
        /// <summary>
        /// 任意一个子节点成功，返回成功。
        /// </summary>
        AnySucceeded = 1,
        /// <summary>
        /// 任意一个子节点完成，返回完成节点的结果。
        /// </summary>
        AnyCompleted = 2,

        /// <summary>
        /// 等待所有子节点都完成，任意一个子节点成功，返回成功。
        /// </summary>
        AnySucceededWaitAll = 3,
        /// <summary>
        /// 等待所有子节点都完成，任意一个子节点失败，返回失败。
        /// </summary>
        AnyFailedWaitAll = 4,
    }

    public class Parallel : CompositeNode
    {
        public FinishMode FinishMode = FinishMode.AnyFailed;

        bool firstTick = false;
        protected override void OnEnter(object options = null)
        {
            firstTick = true;
        }

        protected override Status OnTick(BTNode from, object options = null)
        {
            for (int i = 0; i < Children.Count; i++)
            {
                var child = Children[i];
                if (firstTick == false && child.IsCompleted)
                {
                    //第一次Tick每个子节点都要执行，子节点的状态值是上一次执行的结果。
                    continue;
                }

                child.Tick(this, options);
            }

            firstTick = false;

            var result = CalResultByFinishMode();
            if (result == Status.Succeeded || result == Status.Failed)
            {
                AbortRunningChild();
            }

            return result;
        }

        public Status CalResultByFinishMode()
        {
            switch (FinishMode)
            {
                case FinishMode.AnyFailed:
                    {
                        var hasflag = false;
                        foreach (var child in Children)
                        {
                            if (child.State == Status.Failed)
                            {
                                return child.State;
                            }

                            if (child.State == Status.Running)
                            {
                                hasflag = true;
                            }
                        }

                        return hasflag ? Status.Running : Status.Succeeded;
                    }
                case FinishMode.AnySucceeded:
                    {
                        var hasflag = false;
                        foreach (var child in Children)
                        {
                            if (child.State == Status.Succeeded)
                            {
                                return child.State;
                            }

                            if (child.State == Status.Running)
                            {
                                hasflag = true;
                            }
                        }

                        return hasflag ? Status.Running : Status.Failed;
                    }
                case FinishMode.AnyCompleted:
                    {
                        foreach (var child in Children)
                        {
                            if (child.IsCompleted)
                            {
                                return child.State;
                            }
                        }

                        return Status.Running;
                    }
                case FinishMode.AnySucceededWaitAll:
                    {
                        var hasflag = false;
                        foreach (var child in Children)
                        {
                            if (child.State == Status.Running)
                            {
                                return Status.Running;
                            }

                            if (child.State == Status.Succeeded)
                            {
                                hasflag = true;
                            }
                        }

                        return hasflag ? Status.Succeeded : Status.Failed;
                    }
                case FinishMode.AnyFailedWaitAll:
                    {
                        var hasflag = false;
                        foreach (var child in Children)
                        {
                            if (child.State == Status.Running)
                            {
                                return Status.Running;
                            }

                            if (child.State == Status.Failed)
                            {
                                hasflag = true;
                            }
                        }

                        return hasflag ? Status.Failed : Status.Succeeded;
                    }
                default:
                    return Status.Failed;
            }
        }

        public void AbortRunningChild()
        {
            for (int i = 0; i < Children.Count; i++)
            {
                var child = Children[i];
                if (child.IsCompleted)
                {
                    continue;
                }

                child.Abort(this);
            }
        }


    }
}






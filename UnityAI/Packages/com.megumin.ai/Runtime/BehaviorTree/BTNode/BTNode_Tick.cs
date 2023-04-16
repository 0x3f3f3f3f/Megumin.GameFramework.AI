using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Megumin.GameFramework.AI.BehaviorTree
{
    public partial class BTNode
    {
        /// <summary>
        /// 是不是进入Enter OnTick Exit 域中
        /// </summary>
        public bool IsInnerRunning { get; set; }
        public FailedCode FailedCode { get; set; } = FailedCode.None;
        /// <summary>
        /// 是不是检查过可执行
        /// </summary>
        internal protected bool IsCheckedCanExecute { get; set; }
        /// <summary>
        /// 是不是执行过前置装饰器
        /// </summary>
        internal protected bool IsExecutedPreDecorator { get; set; }
        /// <summary>
        /// 不是执行过Enter
        /// </summary>
        internal protected bool IsExecutedEnter { get; set; }

        /// <summary>
        /// 是不是自身条件中断
        /// </summary>

        internal protected bool abortSelf = false;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="from">当前调用的父节点</param>
        /// <param name="options">预留设计，用于传递可能的Context上下文等参数</param>
        /// <returns></returns>
        /// <remarks>
        /// 使用参数获得父节点，而不是从节点连接关系取得父节点。
        /// 如果行为树文件拓扑结构允许菱形或者环形，可能有多个父节点。
        /// 但是运行时同一时刻只可能有一个调用父节点。
        /// </remarks>
        public Status Tick(BTNode from, object options = null)
        {
            Tree.LastTickNodeIndex = Index;
            Tree.LastTick = this;

            //无论Enabled 是不是true，都要先进入Tick函数再说，不能在外部判断false然后跳过
            //否则在IsRunning期间被改为false，无法触发AbortSelf。
            //AbortSelf 只能由行为树Tick期间调用，因为装饰器等同于节点，不能由外部调用。
            if (Enabled == false)
            {
                if (IsInnerRunning)
                {
                    AbortSelf(options);
                }
                else
                {
                    State = GetIgnoreResult(from);
                    ResetFlag(options);
                }

                return State;
            }

            FailedCode = FailedCode.None;
            //条件阶段
            if (Condition(options) == false)
            {
                //离开节点
                State = Status.Failed;
                if (IsInnerRunning)
                {
                    State = AbortSelf(options);
                }
                else
                {
                    ResetFlag(options);
                }
                return State;
            }

            Execute(from, options);

            return State;
        }

        protected virtual bool Condition(object options = null)
        {
            if (IsCheckedCanExecute)
            {
                return ExecuteConditionDecoratorCheckAbortSelf(options);
            }
            else
            {
                IsCheckedCanExecute = true;
                return CanExecute(options);
            }
        }

        /// <summary>
        /// 当前状态是否完成
        /// </summary>
        public bool IsCompleted => State == Status.Succeeded || State == Status.Failed;

        protected virtual void Execute(BTNode from, object options = null)
        {
            //前置阶段
            if (IsExecutedPreDecorator == false)
            {
                IsExecutedPreDecorator = true;
                State = ExecutePreDecorator(options);
            }

            if (IsCompleted == false)
            {
                Running(from, options);
            }

            //后置阶段 当前已经完成
            if (IsCompleted)
            {
                State = ExecutePostDecorator();
                ResetFlag(options);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Running(BTNode from, object options = null)
        {
            //Enter Exit函数不允许修改State状态。
            //Enter Exit本质是OnTick的拆分，状态始终应该由OnTick决定状态。
            IsInnerRunning = true;
            State = Status.Running;

            if (IsExecutedEnter == false)
            {
                IsExecutedEnter = true;
                Enter(options);
            }

            //OnTick 阶段
            State = OnTick(from, options);

            if (IsCompleted)
            {
                if (IsExecutedEnter)
                {
                    //与enter互斥对应
                    //如果没有调用Enter，那么应不应该调用Exit？
                    Exit(options);
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Enter(object options = null)
        {
            Log($"[{Time.time:0.00}] Enter Node {this}");
            OnEnter(options);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Exit(object options = null)
        {
            OnExit(State, options);
            Log($"[{Time.time:0.00}] Exit Node [{State}]  :  {this}");
        }

        /// <summary>
        /// 离开节点，重置flag
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void ResetFlag(object options = null)
        {
            IsCheckedCanExecute = false;
            IsExecutedPreDecorator = false;
            IsExecutedEnter = false;
            IsInnerRunning = false;
        }

        /// <summary>
        /// 当前节点能否被执行
        /// </summary>
        /// <returns></returns>
        public virtual bool CanExecute(object options = null)
        {
            return ExecuteConditionDecorator(options);
        }

        /// <summary>
        /// 仅仅考虑终止低优先级标记求解值
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// 终止低优先级可能不仅仅受装饰器影响，用户可能会扩展其他功能，所以这里用虚函数包装。
        /// </remarks>
        public virtual bool CanAbortLowerPriority(object options = null)
        {
            return ExecuteConditionDecoratorCheckAbortLowerPriority(options);
        }

        /// <summary>
        /// 当前节点有没有终止低优先级节点标记，需不需要做终止测试
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// 为节点本身也可以需要终止低优先级节点标记预留，用户可以自行扩展。
        /// 实现<see cref="IAbortable"/>接口编辑器UI会自动显示标记。
        /// </remarks>
        public virtual bool HasAbortLowerPriorityFlag(object options = null)
        {
            //TODO 增加version 缓存结果值？
            var hasAbort = Decorators.Any(static elem =>
            {
                return elem is IConditionDecorator conditionable
                        && (conditionable.AbortType & AbortType.LowerPriority) != 0;
            });
            return hasAbort;
        }

        Status AbortSelf(object options = null)
        {
            return Abort(this, options);
        }

        /// <summary>
        /// Abort 理解为当前最后一次不调用Tick的Tick，视为最后通牒。
        /// 中断时不要调用后置装饰器，后置装饰器可能会修改State结果值。仅调用中断装饰器
        /// </summary>
        /// <remarks>
        /// 几乎所有情况都应该返回<see cref="Status.Failed"/>,但是保留返回其他值的可能。
        /// </remarks>
        public Status Abort(BTNode from, object options = null)
        {
            State = Status.Failed;
            FailedCode = FailedCode.Abort;

            OnAbort(options);

            if (IsExecutedEnter)
            {
                //与enter互斥对应
                //如果没有调用Enter，那么应不应该调用Exit？
                Exit(options);
            }

            State = ExecuteAbortDecorator(options);
            ResetFlag(options);
            return State;
        }
    }
}



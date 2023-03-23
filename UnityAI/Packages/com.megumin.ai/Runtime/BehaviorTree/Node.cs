using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using System.Collections;

namespace Megumin.GameFramework.AI.BehaviorTree
{
    /// <summary>
    /// 换个名字，与异步task重名
    /// 
    /// 方法名带不带On,以On开头的方法不应该时public的，不应该由外部类调用。但也不一定。<see cref="StateMachineBehaviour"/>
    /// 
    /// 为了提高性能，成员尽量不要在声明时初始化。
    /// 成员不是在所有情况下都会用到，保持未初始化能有效节省内存。
    /// </summary>
    /// <remarks>
    /// 含有装饰器 等价与 一个父节点 + 前后条件叶子节点 的组合节点。逻辑上视为一个组合节点。
    /// </remarks>
    [Serializable]
    public partial class BTNode : TreeElement
    {
        public NodeMeta Meta;
        /// <summary>
        /// 前置装饰器，没必要分前后，总共也没几个，通过接口判断一下得了
        /// </summary>
        [SerializeReference]
        public List<ITreeElement> Decorators = new();

        /// <summary>
        /// 执行时遇到未开启的节点就忽略。根据父节点返回特定值。
        /// </summary>
        public virtual bool Enabled { get; internal set; } = true;
        public bool IsStarted { get; internal set; }
        public Status State { get; set; } = Status.Init;

        /// <summary>
        /// 节点实例唯一ID
        /// </summary>
        [Space(20)]
        public string InstanceID;




        public ITreeElement AddDecorator(ITreeElement decorator)
        {
            if (!Decorators.Contains(decorator))
            {
                Decorators.Add(decorator);
            }

            return decorator;
        }

        public ITreeElement AddDecorator<T>()
            where T : ITreeElement, new()
        {
            var decorator = new T();
            if (decorator is BTDecorator bTDecorator)
            {
                bTDecorator.GUID = Guid.NewGuid().ToString();
            }
            return AddDecorator(decorator);
        }

        public ITreeElement AddDecorator(Type type)
        {
            var decorator = Activator.CreateInstance(type) as ITreeElement;
            if (decorator is BTDecorator bTDecorator)
            {
                bTDecorator.GUID = Guid.NewGuid().ToString();
            }
            return AddDecorator(decorator);
        }

        internal void RemoveDecorator(ITreeElement decorator)
        {
            Decorators.Remove(decorator);
        }


        internal void Awake()
        {

        }

        internal void Enable()
        {

        }

        internal void Start()
        {

        }

        /// <summary>
        /// 不要再函数内修改State值，会导致流程错误
        /// </summary>
        protected virtual void OnEnter() { }

        /// <summary>
        /// 不要再函数内修改State值，会导致流程错误
        /// </summary>
        /// <param name="result"></param>
        protected virtual void OnExit(Status result) { }

        protected virtual void OnAbort() { }

        protected virtual Status OnTick()
        {
            return Status.Succeeded;
        }
    }

    public partial class BTNode
    {
        [Obsolete("test", true)]
        public async ValueTask<bool> Extest()
        {
            var state = Status.Running;
            while (state != Status.Running)
            {
                //FrontDerators();
                Enter();
                var res = await onticktest();
                //var res2 = Exit(default);
                //res2 = BackDerators(res2);
            }

            return true;
        }

        [Obsolete("test", true)]
        ValueTask<bool> onticktest()
        {
            return new ValueTask<bool>(true);
        }
    }

    public partial class BTNode
    {
        /// <summary>
        /// 是不是进入Enter OnTick Exit 域中
        /// </summary>
        public bool IsInnerRunning { get; set; }
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
        /// <returns></returns>
        /// <remarks>
        /// 使用参数获得父节点，而不是从节点连接关系取得父节点。
        /// 如果行为树文件拓扑结构允许菱形或者环形，可能有多个父节点。
        /// 但是运行时同一时刻只可能有一个调用父节点。
        /// </remarks>
        public Status Tick(BTNode from)
        {
            //无论Enabled 是不是true，都要先进入Tick函数再说，不能在外部判断false然后跳过
            //否则在IsRunning期间被改为false，无法触发AbortSelf。
            //AbortSelf 只能由行为树Tick期间调用，因为装饰器等同于节点，不能由外部调用。
            if (Enabled == false)
            {
                if (IsInnerRunning)
                {
                    AbortSelf();
                }

                //父节点是Selctor 返回Failed，可以允许Selctor 跳过当前节点继续执行下个节点而是直接失败。
                //等同于Ignore。
                if (from is Selector)
                {
                    return Status.Failed;
                }
                return Status.Succeeded;
            }

            //条件阶段
            if (IsCheckedCanExecute == false || abortSelf)
            {
                IsCheckedCanExecute = true;

                var canEnter = CanExecute();
                if (canEnter == false)
                {
                    State = Status.Failed;
                    if (IsInnerRunning)
                    {
                        State = AbortSelf();
                    }
                    return State;
                }
            }

            Execute();

            return State;
        }

        /// <summary>
        /// 当前状态是否完成
        /// </summary>
        bool IsCompleted => State == Status.Succeeded || State == Status.Failed;

        protected void Execute()
        {
            //前置阶段
            if (IsExecutedPreDecorator == false)
            {
                IsExecutedPreDecorator = true;
                State = ExecutePreDeco();
            }

            Running();

            //后置阶段 当前已经完成
            if (IsCompleted)
            {
                State = ExecutePostDeco();
                ResetFlag();
            }
        }

        protected void Running()
        {
            //Enter Exit函数不允许修改State状态。
            //Enter Exit本质是OnTick的拆分，状态始终应该由OnTick决定状态。
            if (IsCompleted == false)
            {
                IsInnerRunning = true;
                State = Status.Running;

                if (IsExecutedEnter == false)
                {
                    IsExecutedEnter = true;
                    Enter();
                }

                //OnTick 阶段
                State = OnTick();

                if (IsCompleted)
                {
                    if (IsExecutedEnter)
                    {
                        //与enter互斥对应
                        //如果没有调用Enter，那么应不应该调用Exit？
                        Exit();
                    }
                }
            }
        }

        void Enter()
        {
            Log($"[{Time.time:0.00}] Enter Node {this.GetType().Name}");
            OnEnter();
        }

        void Exit()
        {
            OnExit(State);
            Log($"[{Time.time:0.00}] Exit Node [{State}]  :  {this.GetType().Name}");
        }

        /// <summary>
        /// 离开节点，重置flag
        /// </summary>
        protected void ResetFlag()
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
        public bool CanExecute()
        {
            return ExecuteConditionDecorator();
        }

        Status AbortSelf()
        {
            return Abort(this);
        }

        /// <summary>
        /// Abort 理解为当前最后一次不调用Tick的Tick，视为最后通牒。
        /// 中断时不要调用后置装饰器，后置装饰器可能会修改State结果值。仅调用中断装饰器
        /// </summary>
        /// <remarks>
        /// 几乎所有情况都应该返回<see cref="Status.Failed"/>,但是保留返回其他值的可能。
        /// </remarks>
        public Status Abort(BTNode from)
        {
            State = Status.Failed;
            OnAbort();

            if (IsExecutedEnter)
            {
                //与enter互斥对应
                //如果没有调用Enter，那么应不应该调用Exit？
                Exit();
            }

            State = ExecuteAbortDeco();
            ResetFlag();
            return State;
        }
    }

    public partial class BTNode
    {
        private bool ExecuteConditionDecorator()
        {
            foreach (var pre in Decorators)
            {
                if (pre is IConditionDecorator conditionable)
                {
                    if (conditionable.Cal() == false)
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
        private Status ExecutePreDeco()
        {
            var res = Status.Running;

            foreach (var pre in Decorators)
            {
                if (pre is IPreDecorator decirator)
                {
                    decirator.BeforeNodeEnter(this);
                }
            }

            return res;
        }

        private Status ExecutePostDeco()
        {
            var res = State;
            //倒序遍历
            for (int i = Decorators.Count - 1; i >= 0; i--)
            {
                var post = Decorators[i];
                if (post is IPostDecorator decirator)
                {
                    res = decirator.AfterNodeExit(res, this);
                }
            }

            return res;
        }

        private Status ExecuteAbortDeco()
        {
            //倒序遍历
            for (int i = Decorators.Count - 1; i >= 0; i--)
            {
                var pre = Decorators[i];
                if (pre is IAbortDecorator decirator)
                {
                    decirator.OnNodeAbort(this);
                }
            }

            return State;
        }
    }
}




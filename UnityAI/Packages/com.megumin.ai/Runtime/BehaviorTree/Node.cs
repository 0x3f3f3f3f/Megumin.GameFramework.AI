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
        public bool Enabled { get; internal set; } = true;
        public bool IsStarted { get; internal set; }
        public Status State { get; set; } = Status.Init;

        /// <summary>
        /// 节点实例唯一ID
        /// </summary>
        [Space(20)]
        public string InstanceID;
        void Enter()
        {
            Log($"[{Time.time:0.00}] Enter Node {this.GetType().Name}");
            State = Status.Running;
            OnEnter();
        }

        protected virtual void OnEnter()
        {

        }

        Status Exit(Status result)
        {
            State = OnExit(result);

            Log($"[{Time.time:0.00}] Exit Node [{State}]  :  {this.GetType().Name}");
            return State;
        }

        protected virtual Status OnExit(Status result)
        {
            return result;
        }



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

        /// <summary>
        /// <para/> Q:为什么CanEnter不放在Tick内部？
        /// <para/> A:当条件中止时需要在不Tick的状态下获取EnterType，放在Tick里会执行2次CanEnter
        /// 
        /// 
        /// TODO：条件装饰器等价与 含有一个Sequence父 + 前面条件叶子节点。
        /// 如果CanEnter返回false，Sequence父逻辑上是失败。
        /// 所以直接视为当前节点返回失败。
        /// 
        /// </summary>
        /// <returns></returns>
        public EnterType CanEnter()
        {
            foreach (var pre in Decorators)
            {
                if (pre is IConditionable conditionable)
                {
                    if (conditionable.Cal() == false)
                    {
                        return EnterType.False;
                    }
                }
            }
            return EnterType.True;
        }

        bool abortSelf = false;

        public Status Tick()
        {
            return Tick2();
            if (State != Status.Running)
            {
                FrontDerators();
                Enter();
            }
            var res = OnTick();
            if (res != Status.Running)
            {
                res = Exit(res);
                res = BackDerators(res);
            }
            return res;
        }



        private void FrontDerators()
        {
            //在Enter之前调用 前置装饰器
            foreach (var pre in Decorators)
            {
                if (pre is IPreDecorator decirator)
                {
                    decirator.BeforeNodeEnter(this);
                }
            }
        }

        private Status BackDerators(Status res)
        {
            //在Exit之后调用 后置装饰器
            //即使remap更改结果，也只影响对上层的返回值，不能也不应该影响Node自身的实际结果值。
            //在Editor上应该表现出node成功，但装饰器返回失败。

            //倒序遍历
            for (int i = Decorators.Count - 1; i >= 0; i--)
            {
                var pre = Decorators[i];
                if (pre is IPostDecorator decirator)
                {
                    res = decirator.AfterNodeExit(res, this);
                    State = res;
                }
            }

            return res;
        }

        /// <summary>
        /// Abort 理解为当前最后一次不调用Tick的Tick，视为最后通牒。
        /// </summary>
        public void Abort()
        {
            OnAbort();
            Exit(Status.Failed);
            BackDerators(Status.Failed);

            //倒序遍历
            for (int i = Decorators.Count - 1; i >= 0; i--)
            {
                var pre = Decorators[i];
                if (pre is IAbortDecorator decirator)
                {
                    decirator.OnNodeAbort(this);
                }
            }
        }

        protected virtual void OnAbort()
        {

        }

        protected virtual Status OnTick()
        {
            return Status.Succeeded;
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

        public async ValueTask<bool> Extest()
        {
            var state = Status.Running;
            while (state != Status.Running)
            {
                FrontDerators();
                Enter();
                var res = await onticktest();
                var res2 = Exit(default);
                res2 = BackDerators(res2);
            }

            return true;
        }

        ValueTask<bool> onticktest()
        {
            return new ValueTask<bool>(true);
        }
    }

    public partial class BTNode
    {
        public bool IsRunning { get; set; }
        bool IsCheckedCanEnter { get; set; }
        /// <summary>
        /// 是不是执行过前置装饰器和Enter
        /// </summary>
        bool IsExcutedPreD { get; set; }
        bool IsCalledEnter { get; set; }
        public Status Tick2()
        {
            //无论Enabled 是不是true，都要先进入Tick函数再说，不能在外部判断false然后跳过
            //否则在IsRunning期间被改为false，无法触发AbortSelf。
            //AbortSelf 只能由行为树Tick期间调用，因为装饰器等同于节点，不能由外部调用。
            if (Enabled == false)
            {
                if (IsRunning)
                {
                    AbortSelf();
                }

                //TODO,父节点是Selctor 返回Failed，等同于Ignore。
                return Status.Succeeded;
            }

            //条件阶段
            if (IsCheckedCanEnter == false || abortSelf)
            {
                var canEnter = CanEnter2();
                if (canEnter == false)
                {
                    State = Status.Failed;
                    if (IsRunning)
                    {
                        State = AbortSelf();
                    }
                    return State;
                }

                IsCheckedCanEnter = true;
            }

            //前置阶段
            if (IsExcutedPreD == false)
            {
                CallPreDeco();
            }

            if (IsCalledEnter == false)
            {
                IsCalledEnter = true;
                Enter();
            }

            //OnTick 阶段
            IsRunning = true;
            if (State == Status.Succeeded || State == Status.Failed)
            {
                //当前已经完成，跳过OnTick
            }
            else
            {
                State = OnTick();
            }

            //后置阶段 当前已经完成
            if (State == Status.Succeeded || State == Status.Failed)
            {
                if (IsCalledEnter)
                {
                    //与enter互斥对应
                    //如果没有调用Enter，那么应不应该调用Exit？
                    State = Exit(State);
                }

                State = CallPostDeco();
                ResetFlag();
            }

            return State;
        }

        public void ResetFlag()
        {
            //离开节点，重置flag
            IsCheckedCanEnter = false;
            IsExcutedPreD = false;
            IsCalledEnter = false;
            IsRunning = false;
        }

        private Status CallPostDeco()
        {
            BackDerators(State);
            return State;
        }

        /// <summary>
        /// 调用前置装饰器
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        private void CallPreDeco()
        {
            IsExcutedPreD = true;
            FrontDerators();

        }

        public bool CanEnter2()
        {
            foreach (var pre in Decorators)
            {
                if (pre is IConditionable conditionable)
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
        /// 中断自身
        /// 中断时不要调用后置装饰器，后置装饰器可能会修改State结果值。仅调用中断装饰器
        /// </summary>
        /// <remarks>
        /// 几乎所有情况都应该返回<see cref="Status.Failed"/>,但是保留返回其他值的可能。
        /// </remarks>
        Status AbortSelf()
        {
            return Status.Failed;
        }


    }
}




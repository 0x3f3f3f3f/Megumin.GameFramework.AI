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
    public class BTNode : TreeElement
    {
        public NodeMeta Meta;
        /// <summary>
        /// 前置装饰器，没必要分前后，总共也没几个，通过接口判断一下得了
        /// </summary>
        [SerializeReference]
        public List<object> Decorators = new();

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
            Debug.Log($"[{Time.time:0.00}] Enter Node {this.GetType().Name}");
            State = Status.Running;
            OnEnter();
        }

        protected virtual void OnEnter()
        {

        }

        Status Exit(Status result)
        {
            State = OnExit(result);

            Debug.Log($"[{Time.time:0.00}] Exit Node [{State}]  :  {this.GetType().Name}");
            return State;
        }

        protected virtual Status OnExit(Status result)
        {
            return result;
        }

        

        public object AddDecorator(object decorator)
        {
            if (!Decorators.Contains(decorator))
            {
                Decorators.Add(decorator);
            }

            return decorator;
        }

        public object AddDecorator<T>()
            where T : class, new()
        {
            var decorator = new T();
            if (decorator is BTDecorator bTDecorator)
            {
                bTDecorator.GUID = Guid.NewGuid().ToString();
            }
            return AddDecorator(decorator);
        }

        public object AddDecorator(Type type)
        {
            var decorator = Activator.CreateInstance(type);
            if (decorator is BTDecorator bTDecorator)
            {
                bTDecorator.GUID = Guid.NewGuid().ToString();
            }
            return AddDecorator(decorator);
        }

        internal void RemoveDecorator(object decorator)
        {
            Decorators.Remove(decorator);
        }

        /// <summary>
        /// <para/> Q:为什么CanEnter不放在Tick内部？
        /// <para/> A:当条件中止时需要在不Tick的状态下获取EnterType，放在Tick里会执行2次CanEnter
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

        public Status Tick()
        {
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
                if (pre is IPreDecirator decirator)
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
                if (pre is IPostDecirator decirator)
                {
                    res = decirator.AfterNodeExit(res, this);
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
                if (pre is IAbortDecirator decirator)
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
}




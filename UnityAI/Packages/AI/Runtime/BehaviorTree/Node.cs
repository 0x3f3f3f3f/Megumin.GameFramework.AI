using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Megumin.GameFramework.AI.BehaviorTree
{
    /// <summary>
    /// 换个名字，与异步task重名
    /// 
    /// 方法名带不带On,以On开头的方法不应该时public的，不应该由外部类调用。
    /// 
    /// 为了提高性能，成员尽量不要在声明时初始化。
    /// 成员不是在所有情况下都会用到，保持未初始化能有效节省内存。
    /// </summary>
    public class BTNode
    {
        /// <summary>
        /// tree中唯一ID。子树中可能会出现冲突，使用GUID.ToString 尽量保证唯一性。
        /// </summary>
        string UID;
        /// <summary>
        /// 运行时进程空间唯一ID。
        /// </summary>
        string GUID;

        public bool Enabled { get; internal set; }
        public bool IsStarted { get; internal set; }
        public Status State { get; set; } = Status.Init;

        void Enter()
        {
            Debug.Log($"Enter Node {this.GetType().Name}");
            State = Status.Running;
            OnEnter();


        }

        protected virtual void OnEnter()
        {

        }

        Status Exit(Status result)
        {
            State = OnExit(result);
            Debug.Log($"Exit Node {this.GetType().Name}");
            return State;
        }

        protected virtual Status OnExit(Status result)
        {
            return result;
        }

        /// <summary>
        /// 前置装饰器
        /// </summary>
        public object[] perDerator;

        /// <summary>
        /// <para/> Q:为什么CanEnter不放在Tick内部？
        /// <para/> A:当条件中止时需要在不Tick的状态下获取EnterType，放在Tick里会执行2次CanEnter
        /// </summary>
        /// <returns></returns>
        public EnterType CanEnter()
        {
            if (perDerator?.Length > 0)
            {
                foreach (var pre in perDerator)
                {
                    if (pre is IConditionable conditionable)
                    {
                        if (conditionable.Cal() == false)
                        {
                            return EnterType.False;
                        }
                    }
                }
            }
            return EnterType.True;
        }

        public Status Tick()
        {
            if (State != Status.Running)
            {
                Enter();
            }
            var res = OnTick();
            if (res != Status.Running)
            {
                res = Exit(res);
            }
            return res;
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
    }
}




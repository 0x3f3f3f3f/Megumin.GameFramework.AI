using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using System.Collections;
using System.Runtime.CompilerServices;
using Megumin.Reflection;

namespace Megumin.AI.BehaviorTree
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
    public partial class BTNode : BehaviorTreeElement
    {
        public NodeMeta Meta;
        /// <summary>
        /// 执行时遇到未开启的节点就忽略。根据父节点返回特定值。
        /// </summary>
        [field: SerializeField]
        public bool Enabled { get; internal set; } = true;

        /// <summary>
        /// 前置装饰器，没必要分前后，总共也没几个，通过接口判断一下得了
        /// </summary>
        [SerializeReference]
        [SetMemberBy("SetDecorators")]
        public List<IDecorator> Decorators = new();

        protected bool SetDecorators(object value)
        {
            if (value is List<IDecorator> ds)
            {
                Decorators = ds;
                return true;
            }
            else if (value is List<ITreeElement> old)
            {
                Debug.Log($"Use MyTest");
                foreach (ITreeElement e in old)
                {
                    if (e is IDecorator decorator)
                    {
                        AddDecorator(decorator);
                    }
                }

                return true;
            }

            return false;
        }

        public Status State { get; set; } = Status.Init;

        /// <summary>
        /// 节点实例唯一ID
        /// </summary>
        public string InstanceID { get; set; }

        public IDecorator AddDecorator(IDecorator decorator)
        {
            if (!Decorators.Contains(decorator))
            {
                Decorators.Add(decorator);
                decorator.Owner = this;
            }

            return decorator;
        }

        public void RemoveDecorator(IDecorator decorator)
        {
            Decorators.Remove(decorator);
            if (decorator.Owner == this)
            {
                decorator.Owner = null;
            }
        }

        public IDecorator AddDecorator<T>()
            where T : IDecorator, new()
        {
            var decorator = new T();
            if (decorator is BTDecorator bTDecorator)
            {
                bTDecorator.GUID = Guid.NewGuid().ToString();
            }
            return AddDecorator(decorator);
        }

        public IDecorator AddDecorator(Type type)
        {
            var decorator = Activator.CreateInstance(type) as IDecorator;
            if (decorator is BTDecorator bTDecorator)
            {
                bTDecorator.GUID = Guid.NewGuid().ToString();
            }
            return AddDecorator(decorator);
        }

        /// <summary>
        /// 第一次执行进入节点
        /// </summary>
        /// <remarks>
        /// 可以在方法内部修改State值，允许在不经过OnTick阶段的情况下直接完成节点。
        /// </remarks>
        protected virtual void OnEnter(BTNode from, object options = null) { }

        /// <summary>
        /// 第一次执行进入节点,并返回状态值
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        protected virtual Status OnEnter2(BTNode from, object options = null)
        {
            return State;
        }

        /// <summary>
        /// 退出节点时调用
        /// </summary>
        /// <remarks>
        /// 不建议在方法内部修改State值，可能会导致流程错误
        /// </remarks>
        protected virtual void OnExit(BTNode from, Status result, object options = null) { }

        /// <summary>
        /// 节点被终止时调用
        /// </summary>
        /// <param name="options"></param>
        protected virtual void OnAbort(BTNode from, object options = null) { }

        /// <summary>
        /// 轮询子类
        /// </summary>
        /// <param name="from"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        protected virtual Status OnTick(BTNode from, object options = null)
        {
            //这里默认返回Running。当用户使用OnTickAsync，并且不重写OnTick时，保证节点不会意外结束。
            return Status.Running;
        }

        /// <summary>
        /// 异步Tick。需要设置<seealso cref="TickMode"/>。
        /// </summary>
        /// <returns></returns>
        protected virtual ValueTask<bool> OnTickAsync(BTNode from, object options = null)
        {
            return new ValueTask<bool>(result: true);
        }

        /// <summary>
        /// 根据调用节点返回不同的结果值，使调用节点(from)忽略当前节点。
        /// <para/> 父节点是Selctor 返回Failed，可以允许Selctor 跳过当前节点继续执行下个节点而是直接失败。
        /// <para/> 使用这个函数，而不是<![CDATA[from.GetResultIfChildIgnored(this)]]>, 因为from可能为null导致空引用。
        /// </summary>
        /// <param name="from"></param>
        /// <returns></returns>
        public virtual Status GetIgnoreResult(BTNode from)
        {
            //避免这里引用Selector节点
            //if (from is Selector)
            //{
            //    return Status.Failed;
            //}
            //else
            //{
            //    return Status.Succeeded;
            //}

            if (from == null)
            {
                return Status.Succeeded;
            }
            return from.GetResultIfChildIgnored(this);
        }

        /// <summary>
        /// <para/> 当子节点失效时，忽略时，无法正常工作时，这个节点作为父节点(from) ，期待返回的最合适结果。
        /// 这个结果作为子节点的执行结果。
        /// <para/> 仅重写这个函数，不要调用这个函数。请使用<see cref="GetIgnoreResult(BTNode)"/>
        /// </summary>
        /// <param name="from"></param>
        /// <returns>
        /// 可以让当前节点可以继续执行的结果值
        /// </returns>
        /// <remarks>
        /// 通常都期待子节点返回成功。而Selector期待子节点返回失败，这样才能继续执行下一个子节点。
        /// </remarks>
        protected virtual Status GetResultIfChildIgnored(BTNode ignoredChild)
        {
            return Status.Succeeded;
        }

        public bool TryGetFirstParent(out BTParentNode parentNode)
        {
            if (Tree != null)
            {
                return Tree.TryGetFirstParent(this, out parentNode);
            }

            parentNode = null;
            return false;
        }

        /// <summary>
        /// 获取执行路径
        /// </summary>
        /// <param name="exetutePath"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public bool TryGetFirstExetutePath(List<BTParentNode> exetutePath)
        {
            if (Tree != null)
            {
                return Tree.TryGetFirstExetutePath(this, exetutePath);
            }

            return false;
        }
    }
}




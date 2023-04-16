using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using System.Collections;
using System.Runtime.CompilerServices;
using Megumin.Reflection;

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


        //internal void Awake()
        //{

        //}

        //internal void Enable()
        //{

        //}

        //internal void Start()
        //{

        //}

        public void Reset(object options = null)
        {
            OnReset(options);
        }
        /// <summary>
        /// 不要再函数内修改State值，会导致流程错误
        /// </summary>
        protected virtual void OnEnter(object options = null) { }

        /// <summary>
        /// 不要再函数内修改State值，会导致流程错误
        /// </summary>
        /// <param name="result"></param>
        protected virtual void OnExit(Status result, object options = null) { }

        protected virtual void OnAbort(object options = null) { }

        protected virtual Status OnTick(BTNode from, object options = null)
        {
            return Status.Succeeded;
        }

        protected virtual void OnReset(object options = null) { }

        /// <summary>
        /// 根据调用节点返回不同的结果值，使调用节点忽略当前节点。
        /// <para/> 父节点是Selctor 返回Failed，可以允许Selctor 跳过当前节点继续执行下个节点而是直接失败。
        /// </summary>
        /// <param name="from"></param>
        /// <returns></returns>
        protected Status GetIgnoreResult(BTNode from)
        {
            if (from is Selector)
            {
                return Status.Failed;
            }
            else
            {
                return Status.Succeeded;
            }
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




using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Serialization;

namespace Megumin.AI.BehaviorTree
{
    public abstract class BTParentNode : BTNode
    {
        /// <summary>
        /// 这里必须使用泛型序列化，否则Undo/Redo 时元素会丢失自己的真实类型。notconnect 多层级颜色bug
        /// </summary>
        [HideInInspector]
        [SerializeReference]
        [FormerlySerializedAs("children")]
        public List<BTNode> Children = new();

        /// <summary>
        /// 条件终止 动态模式
        /// </summary>
        [Tooltip("It is recommended to use AbortType instead of Dynamic.")]
        public bool Dynamic = false;

        public bool ContainsChild(BTNode node)
        {
            foreach (BTNode child in Children)
            {
                if (child.GUID == node.GUID)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 测试一个节点是不是自己的子代
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public bool IsDescendant(BTNode node, List<BTParentNode> parentPath = null)
        {
            if (node == null)
            {
                return false;
            }

            foreach (BTNode child in Children)
            {
                if (child.GUID == node.GUID)
                {
                    parentPath?.Add(this);
                    return true;
                }

                if (child is BTParentNode parentNode)
                {
                    var result = parentNode.IsDescendant(node, parentPath);
                    if (result)
                    {
                        parentPath?.Add(this);
                        return true;
                    }
                }
            }

            return false;
        }
    }

    public abstract class CompositeNode : BTParentNode
    {
        public int CurrentIndex { get; protected set; } = -1;

        protected override void OnEnter(object options = null)
        {
            CurrentIndex = 0;
        }

        protected override void OnAbort(object options = null)
        {
            foreach (var item in Children)
            {
                if (item.State == Status.Running)
                {
                    item.Abort(this, options);
                }
            }
        }
    }

    /// <summary>
    /// 在其他行为树模型中，也可以称为装饰器节点。
    /// 为了避免和附加的装饰器混淆，这里不命名为装饰器。
    /// </summary>
    public abstract class OneChildNode : BTParentNode
    {
        public BTNode Child0
        {
            get
            {
                if (Children.Count > 0)
                {
                    return Children[0];
                }
                else
                {
                    return null;
                }
            }
        }

        protected override void OnAbort(object options = null)
        {
            Child0.Abort(this, options);
        }
    }

    public abstract class TwoChildNode : BTParentNode
    {
        public BTNode Child0
        {
            get
            {
                if (Children.Count > 0)
                {
                    return Children[0];
                }
                else
                {
                    return null;
                }
            }
        }

        public BTNode Child1
        {
            get
            {
                if (Children.Count > 1)
                {
                    return Children[1];
                }
                else
                {
                    return null;
                }
            }
        }

        protected override void OnAbort(object options = null)
        {
            Child0.Abort(this, options);
        }
    }


    //父节点支持泛型组件

    public abstract class BTParentNode<T> : BTParentNode, IHasMyAgent, IMyAgentable<T>
    {
        public T MyAgent { [DebuggerStepThrough] get; [DebuggerStepThrough] set; }

        //如果T为接口，组件不能实现接口，使用代理类型。
        //实际使用发现，编写代理类型代码，和编写一个独立节点工作量差不多。
        //性能也会损失，所以放弃这个实现。
        //public string AdpterType = null;

        /// <summary>
        /// 验证MyAgent有效性，防止Tick过程中空引用异常
        /// </summary>
        [Space]
        [Tooltip("Verify MyAgent validity to prevent NullReferenceException in Tick process")]
        [FormerlySerializedAs("SafeMyAgent")]
        public bool VerifyMyAgent = true;

        /// <summary>
        /// 有时候MyAgent初始化晚于行为树，可能导致MyAgent组件无法获得。
        /// 这个开关用于在节点执行时重新BindMyAgent。
        /// </summary>
        [FormerlySerializedAs("RebindMyAgentBeforeCanExecute")]
        [Tooltip("Rebind myAgent before CanExecute. More performance overhead")]
        public bool AutoRebind = false;

        public bool HasMyAgent()
        {
            if (MyAgent == null)
            {
                return false;
            }

            if (MyAgent is UnityEngine.Object obj && !obj)
            {
                return false;
            }
            return true;
        }

        public override void BindAgent(object agent)
        {
            base.BindAgent(agent);

            BindMyAgent(true);
        }

        protected virtual void BindMyAgent(bool force = false)
        {
            if (force || HasMyAgent() == false)
            {
                if (Agent is T tAgent)
                {
                    MyAgent = tAgent;
                }
                else
                {
                    if (GameObject)
                    {
                        MyAgent = GameObject.GetComponentInChildren<T>();
                    }
                }
            }
        }

        public override bool CanExecute(object options = null)
        {
            if (AutoRebind)
            {
                BindMyAgent();
            }

            if (VerifyMyAgent)
            {
                if (HasMyAgent() == false)
                {
                    return false;
                }
            }

            return base.CanExecute(options);
        }
    }

    public abstract class CompositeNode<T> : BTParentNode<T>
    {
        public int CurrentIndex { get; protected set; } = -1;

        protected override void OnEnter(object options = null)
        {
            CurrentIndex = 0;
        }

        protected override void OnAbort(object options = null)
        {
            foreach (var item in Children)
            {
                if (item.State == Status.Running)
                {
                    item.Abort(this, options);
                }
            }
        }
    }

    public abstract class OneChildNode<T> : BTParentNode<T>
    {
        public BTNode Child0
        {
            get
            {
                if (Children.Count > 0)
                {
                    return Children[0];
                }
                else
                {
                    return null;
                }
            }
        }

        protected override void OnAbort(object options = null)
        {
            Child0.Abort(this, options);
        }
    }

    public abstract class TwoChildNode<T> : BTParentNode<T>
    {
        public BTNode Child0
        {
            get
            {
                if (Children.Count > 0)
                {
                    return Children[0];
                }
                else
                {
                    return null;
                }
            }
        }

        public BTNode Child1
        {
            get
            {
                if (Children.Count > 1)
                {
                    return Children[1];
                }
                else
                {
                    return null;
                }
            }
        }

        protected override void OnAbort(object options = null)
        {
            Child0.Abort(this, options);
        }
    }


    /// <summary>
    /// 根据特定状态执行Child0。看作一个小状态机。
    /// <para/>
    /// 近似装饰节点DecoratorNode，是一种特殊的OneChildNode。
    /// 但称为装饰节点并不合适，因为不是以修饰子节点为主要目的的。
    /// 它自身的逻辑可能不通用，并且同样十分主要，不能看作一个装饰。
    /// </summary>
    public abstract class StateChild0 : OneChildNode
    {
        protected override void OnEnter(object options = null)
        {
            base.OnEnter(options);
            InChild = false;
        }

        protected bool InChild = false;
        protected override Status OnTick(BTNode from, object options = null)
        {
            if (InChild)
            {
                //执行子节点
                var childResult = Child0?.Tick(this, options);
                if (childResult == Status.Running)
                {
                    return Status.Running;
                }

                //子节点完成，由子类决定继续执行还是完成。
                InChild = false;
                return OnChildComplete(childResult);
            }
            else
            {
                //自身执行部分，决定是否进入子节点，还是完成自身。
                var (ChangeTo, Result) = OnTickSelf(from, options);
                if (ChangeTo)
                {
                    InChild = true;
                    return Status.Running;
                }
                else
                {
                    return Result;
                }
            }
        }

        /// <summary>
        /// 父节点自身的逻辑，决定是否进入子节点，还是完成自身。
        /// </summary>
        /// <param name="from"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public abstract (bool ChangeTo, Status Result) OnTickSelf(BTNode from, object options = null);

        /// <summary>
        /// 子节点为null或者完成时被调用。
        /// 由子类决定继续执行还是完成。
        /// </summary>
        /// <param name="childResult"></param>
        /// <returns></returns>
        public abstract Status OnChildComplete(Status? childResult);
    }

    /// <summary>
    /// 根据特定状态执行Child0。看作一个小状态机。
    /// <para/>
    /// 近似装饰节点DecoratorNode，是一种特殊的OneChildNode。
    /// 但称为装饰节点并不合适，因为不是以修饰子节点为主要目的的。
    /// 它自身的逻辑可能不通用，并且同样十分主要，不能看作一个装饰。
    /// </summary>
    public abstract class StateChild0<T> : OneChildNode<T>
    {
        protected override void OnEnter(object options = null)
        {
            base.OnEnter(options);
            InChild = false;
        }

        protected bool InChild = false;
        protected override Status OnTick(BTNode from, object options = null)
        {
            if (InChild)
            {
                //执行子节点
                var childResult = Child0?.Tick(this, options);
                if (childResult == Status.Running)
                {
                    return Status.Running;
                }

                //子节点完成，由子类决定继续执行还是完成。
                InChild = false;
                return OnChildComplete(childResult);
            }
            else
            {
                //自身执行部分，决定是否进入子节点，还是完成自身。
                var (ChangeTo, Result) = OnTickSelf(from, options);
                if (ChangeTo)
                {
                    InChild = true;
                    return Status.Running;
                }
                else
                {
                    return Result;
                }
            }
        }

        /// <summary>
        /// 父节点自身的逻辑，决定是否进入子节点，还是完成自身。
        /// </summary>
        /// <param name="from"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public abstract (bool ChangeTo, Status Result) OnTickSelf(BTNode from, object options = null);

        /// <summary>
        /// 子节点为null或者完成时被调用。
        /// 由子类决定继续执行还是完成。
        /// </summary>
        /// <param name="childResult"></param>
        /// <returns></returns>
        public abstract Status OnChildComplete(Status? childResult);
    }

    [Obsolete("失败设计，当otherNode为null时，OnChildComp无法被调用", true)]
    public abstract class PassThrough : OneChildNode
    {
        protected override void OnEnter(object options = null)
        {
            base.OnEnter(options);
            otherNode = null;
        }

        protected BTTaskNode otherNode = null;
        //protected override Status OnTick(BTNode from, object options = null)
        //{
        //    if (otherNode != null)
        //    {
        //        //执行子节点
        //        var childResult = Child0?.Tick(this, options);
        //        if (childResult == Status.Running)
        //        {
        //            return Status.Running;
        //        }

        //        //子节点完成，由子类决定继续执行还是完成。
        //        otherNode = null;
        //        return OnChildComp(childResult);
        //    }
        //    else
        //    {
        //        //自身执行部分，决定是否进入子节点，还是完成自身。
        //        var (ChangeTo, Result) = OnTickSelf(from, options);
        //        if (ChangeTo != null)
        //        {
        //            InChild = true;
        //            return Status.Running;
        //        }
        //        else
        //        {
        //            return Result;
        //        }
        //    }
        //}

        public abstract (BTTaskNode otherNode, Status Result) OnTickSelf(BTNode from, object options = null);
        /// <summary>
        /// 子节点为null或者完成时被调用。
        /// 由子类决定继续执行还是完成。
        /// </summary>
        /// <param name="childResult"></param>
        /// <returns></returns>
        public abstract Status OnOtherNode(Status? childResult);
    }

}




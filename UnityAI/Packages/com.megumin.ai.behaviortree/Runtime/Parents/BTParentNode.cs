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
    /// 交替执行自身和子节点
    /// </summary>
    [Obsolete("还没设计好",true)]
    public abstract class Iterator : OneChildNode
    { 
        protected bool RunChild(object options = null)
        {
            if (runchildMode == false || Child0 == null)
            {
                return false;
            }
            else
            {
                var result = Child0.Tick(this, options);
                if (result == Status.Running)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

        }

        public bool DoMyself()
        {
            return false;
        }

        bool runchildMode = false;

        public int CurrentIndex { get; protected set; } = -1;

        protected override Status OnTick(BTNode from, object options = null)
        {
            if (CurrentIndex == 0)
            {
                if (Child0 != null)
                {
                    var childResult = Child0.Tick(this, options);
                    if (true)
                    {

                    }
                }

            }
            else
            {
                
            }

           
            if (RunChild(options))
            {
                return Status.Running;
            }
            else
            {
                if (DoMyself())
                {
                    runchildMode = true;
                    return Status.Running;
                }
            }


            return base.OnTick(from, options);
        }
    }

}




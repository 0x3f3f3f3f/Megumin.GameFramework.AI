using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Megumin.GameFramework.AI.BehaviorTree
{
    [Serializable]
    public class BTTaskNode : BTNode
    {

    }

    [Serializable]
    public class BTActionNode : BTTaskNode
    {

    }

    /// <summary>
    /// 泛型节点，方便获取泛型Agent。
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BTActionNode<T> : BTActionNode
    {
        public T MyAgent { get; set; }

        /// <summary>
        /// 有时候MyAgent初始化晚于行为树，可能导致MyAgent组件无法获得。
        /// 这个开关用于在节点执行时重新BindMyAgent。
        /// </summary>
        [Space]
        public bool RebindMyAgentBeforeCanExecute = false;

        /// <summary>
        /// 验证MyAgent有效性，防止Tick过程中空引用异常
        /// </summary>
        [Tooltip("Verify MyAgent validity to prevent hollow reference exceptions in Tick process")]
        public bool SafeMyAgent = true;

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
            if (RebindMyAgentBeforeCanExecute)
            {
                BindMyAgent();
            }

            if (SafeMyAgent)
            {
                if (HasMyAgent() == false)
                {
                    return false;
                }
            }

            return base.CanExecute(options);
        }
    }

    /// <summary>
    /// 使用条件装饰器代替条件任务节点
    /// </summary>
    [Serializable]
    public class BTConditionNode : BTTaskNode
    {
        protected sealed override Status OnTick(BTNode from, object options = null)
        {
            return CheckCondition(from) ? Status.Succeeded : Status.Failed;
        }

        public bool Invert = false;

        //[field: SerializeField]
        //public AbortType AbortType { get; set; }

        public bool LastCheckResult { get; protected set; }

        //每帧只求解一次。TODO，还没有考虑好要不要加
        //public int LastCheckTickCount { get; protected set; } = -1;
        //public bool CalOnceOnTick = false;

        public bool CheckCondition(BTNode from)
        {
            //if (CalOnceOnTick)
            //{
            //    if (Tree.TotalTickCount == LastCheckTickCount)
            //    {
            //        return LastCheckResult;
            //    }
            //}
            //LastCheckTickCount = Tree.TotalTickCount;

            LastCheckResult = OnCheckCondition(from);

            if (Invert)
            {
                LastCheckResult = !LastCheckResult;
            }

            return LastCheckResult;
        }

        protected virtual bool OnCheckCondition(BTNode from)
        {
            return false;
        }
    }
}

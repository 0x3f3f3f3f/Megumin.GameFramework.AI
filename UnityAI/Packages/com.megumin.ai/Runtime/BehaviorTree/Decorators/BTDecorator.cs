using System;
using UnityEngine;

namespace Megumin.GameFramework.AI.BehaviorTree
{
    public class BTDecorator : BehaviorTreeElement, IDecorator
    {
        public BTNode Owner { get; set; }
    }

    public class BTDecorator<T> : BTDecorator
    {
        public T MyAgent { get; set; }

        public override void BindAgent(object agent)
        {
            base.BindAgent(agent);
            if (agent is T tAgent)
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

    /// <summary>
    /// 条件装饰器
    /// </summary>
    public class ConditionDecorator : BTDecorator, IAbortable, IConditionDecorator
    {
        public bool Invert = false;

        [field: SerializeField]
        public AbortType AbortType { get; set; }

        public bool LastCheckResult { get; protected set; }

        //每帧只求解一次。TODO，还没有考虑好要不要加
        //public int LastCheckTickCount { get; protected set; } = -1;
        //public bool CalOnceOnTick = false;

        public virtual bool CheckCondition(object options = null)
        {
            //if (CalOnceOnTick)
            //{
            //    if (Tree.TotalTickCount == LastCheckTickCount)
            //    {
            //        return LastCheckResult;
            //    }
            //}
            //LastCheckTickCount = Tree.TotalTickCount;

            LastCheckResult = OnCheckCondition(options);

            if (Invert)
            {
                LastCheckResult = !LastCheckResult;
            }

            return LastCheckResult;
        }

        protected virtual bool OnCheckCondition(object options = null)
        {
            return false;
        }
    }

    public class ConditionDecorator<T> : ConditionDecorator
    {
        public T MyAgent { get; set; }

        public override void BindAgent(object agent)
        {
            base.BindAgent(agent);
            if (agent is T tAgent)
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

        /// <summary>
        /// 验证MyAgent有效性，防止Tick过程中空引用异常
        /// </summary>
        public bool CheckMyAgent = true;

        public override bool CheckCondition(object options = null)
        {
            if (CheckMyAgent)
            {
                if (MyAgent == null)
                {
                    return false;
                }

                if (MyAgent is UnityEngine.Object obj && !obj)
                {
                    return false;
                }
            }

            return base.CheckCondition(options);
        }
    }


    public abstract class CompareDecorator<V> : ConditionDecorator
        where V : IComparable<V>
    {
        public CompareToMode Mode = CompareToMode.Equals;

        public abstract V GetResult();

        public abstract V GetCompareTo();

        protected override bool OnCheckCondition(object options = null)
        {
            var compareResult = GetResult().CompareTo(GetCompareTo());
            switch (Mode)
            {
                case CompareToMode.Equals:
                    return compareResult == 0;
                case CompareToMode.Less:
                    return compareResult < 0;
                case CompareToMode.Greater:
                    return compareResult > 0;
                case CompareToMode.LessEquals:
                    return compareResult <= 0;
                case CompareToMode.GreaterEquals:
                    return compareResult >= 0;
                case CompareToMode.NotEqual:
                    return compareResult != 0;
                default:
                    break;
            }

            return base.OnCheckCondition(options);
        }
    }

    public abstract class CompareDecorator<T, V> : ConditionDecorator<T>
         where V : IComparable<V>
    {
        public CompareToMode Mode = CompareToMode.Equals;

        public abstract V GetResult();

        public abstract V GetCompareTo();

        protected override bool OnCheckCondition(object options = null)
        {
            var compareResult = GetResult().CompareTo(GetCompareTo());
            switch (Mode)
            {
                case CompareToMode.Equals:
                    return compareResult == 0;
                case CompareToMode.Less:
                    return compareResult < 0;
                case CompareToMode.Greater:
                    return compareResult > 0;
                case CompareToMode.LessEquals:
                    return compareResult <= 0;
                case CompareToMode.GreaterEquals:
                    return compareResult >= 0;
                case CompareToMode.NotEqual:
                    return compareResult != 0;
                default:
                    break;
            }

            return base.OnCheckCondition(options);
        }
    }
}




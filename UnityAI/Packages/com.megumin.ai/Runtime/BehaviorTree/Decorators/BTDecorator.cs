using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public bool CheckCondition()
        {
            //if (CalOnceOnTick)
            //{
            //    if (Tree.TotalTickCount == LastCheckTickCount)
            //    {
            //        return LastCheckResult;
            //    }
            //}
            //LastCheckTickCount = Tree.TotalTickCount;

            LastCheckResult = OnCheckCondition();

            if (Invert)
            {
                LastCheckResult = !LastCheckResult;
            }

            return LastCheckResult;
        }

        protected virtual bool OnCheckCondition()
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
    }
}




﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public override bool CanExecute()
        {
            if (MyAgent == null)
            {
                return false;
            }

            if (MyAgent is UnityEngine.Object obj && !obj)
            {
                return false;
            }

            return base.CanExecute();
        }

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
    /// 使用条件装饰器代替条件任务节点
    /// </summary>
    [Serializable]
    public class BTConditionNode : BTTaskNode
    {
        protected sealed override Status OnTick(BTNode from)
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

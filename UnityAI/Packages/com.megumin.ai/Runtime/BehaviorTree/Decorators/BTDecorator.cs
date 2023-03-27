using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Megumin.GameFramework.AI.BehaviorTree
{
    public class BTDecorator : TreeElement, IDecorator
    {
    }

    /// <summary>
    /// 条件装饰器
    /// </summary>
    public class ConditionDecorator : BTDecorator, IAbortable, IConditionDecorator
    {
        [field: SerializeField]
        public AbortType AbortType { get; set; }

        public bool CheckCondition()
        {
            LastCheckResult = OnCheckCondition();
            return LastCheckResult;
        }

        protected virtual bool OnCheckCondition()
        {
            return false;
        }

        public bool LastCheckResult { get; protected set; }
    }
}




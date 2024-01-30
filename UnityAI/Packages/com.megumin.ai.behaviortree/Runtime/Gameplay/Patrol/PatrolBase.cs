using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Megumin.Binding;
using UnityEngine;

namespace Megumin.AI.BehaviorTree
{
    /// <summary>
    /// 巡逻基类
    /// 每到达一个检查点，执行一次子节点。
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class PatrolBase<T, P> : StateChild0<T>, IOutputPortInfoy<string>
        where P : PatrolPath<Vector3>
    {
        [Space]
        public ArriveChecker ArriveChecker = new();

        /// <summary>
        /// 巡逻路径
        /// </summary>
        [Space]
        public P PatrolPath;

        protected Vector3 startPosition;

        protected override void OnAwake(object options = null)
        {
            base.OnAwake(options);
            startPosition = Transform.position;
            PatrolPath.StartPoint = startPosition;
        }

        protected Vector3 destination;

        public string OutputPortInfo => "OnArrivedCheckPoint";

        protected override void OnEnter(BTNode from, object options = null)
        {
            base.OnEnter(from, options);
            ArriveChecker.CalStopingDistance(GameObject, null);
        }
    }
}


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using System.Diagnostics;

namespace Megumin.AI.BehaviorTree
{
    [Serializable]
    public class BehaviorTreeElement : TreeElement<BehaviorTree>,
        IBindAgentable,
        IAgentable,
        IAwakeable,
        IStartable,
        IResetable
    {
        public object Agent { [DebuggerStepThrough] get; [DebuggerStepThrough] set; }

        public GameObject GameObject { [DebuggerStepThrough] get; [DebuggerStepThrough] set; }

        public Transform Transform
        {
            [DebuggerStepThrough]
            get
            {
                return GameObject == null ? null : GameObject.transform;
            }
        }

        public virtual void BindAgent(object agent)
        {
            Agent = agent;

            if (agent is Component component)
            {
                GameObject = component.gameObject;
            }
            else
            {
                GameObject = agent as GameObject;
            }
        }

        public void Awake(object options = null)
        {
            OnAwake(options);
        }

        /// <summary>
        /// 绑定之后，解析之后，第一次Tick开始时调用，不能保证所有节点的调用顺序
        /// </summary>
        /// <param name="options"></param>
        protected virtual void OnAwake(object options = null)
        {

        }

        public void Start(object options = null)
        {
            OnStart(options);
        }

        /// <summary>
        /// 绑定之后，解析之后，第一次Tick开始时，Awake之后调用，不能保证所有节点的调用顺序
        /// </summary>
        /// <param name="options"></param>
        protected virtual void OnStart(object options = null)
        {

        }

        public void Reset(object options = null)
        {
            OnReset(options);
        }

        protected virtual void OnReset(object options = null)
        {

        }
    }
}






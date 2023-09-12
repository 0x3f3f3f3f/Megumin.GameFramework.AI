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
    public class BehaviorTreeElement : TreeElement<BehaviorTree>, IBindAgentable
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
    }
}






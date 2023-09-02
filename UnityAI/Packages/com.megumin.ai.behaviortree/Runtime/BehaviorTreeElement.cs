using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Megumin.AI.BehaviorTree
{
    [Serializable]
    public class BehaviorTreeElement : TreeElement<BehaviorTree>, IBindAgentable
    {
        public object Agent { get; set; }
        public GameObject GameObject { get; set; }
        public Transform Transform => GameObject == null ? null : GameObject.transform;

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






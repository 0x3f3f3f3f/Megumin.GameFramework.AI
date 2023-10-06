using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Megumin.AI.BehaviorTree
{
    public partial class BehaviorTree : IBindAgentable
    {
        public object Agent { get; set; }
        public GameObject GameObject { get; set; }
        public Transform Transform => GameObject != null ? GameObject.transform : null;

        public HashSet<IBindAgentable> AllBindAgentable { get; } = new();

        protected static readonly Unity.Profiling.ProfilerMarker BindAgentMarker = new("BindAgent");

        public virtual void BindAgent(object agent)
        {
            using var profiler = BindAgentMarker.Auto();

            Agent = agent;

            if (agent is Component component)
            {
                GameObject = component.gameObject;
            }
            else
            {
                GameObject = agent as GameObject;
            }

            foreach (var item in AllBindAgentable)
            {
                item.BindAgent(agent);
            }
        }
    }
}



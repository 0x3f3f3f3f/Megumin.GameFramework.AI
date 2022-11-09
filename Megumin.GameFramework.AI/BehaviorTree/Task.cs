using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace Megumin.GameFramework.AI.BehaviorTree
{
    public class Task
    {
        public TraceListener TraceListener { get; set; }

        public string Name { get; set; }
        public object Agent { get; set; }
        public static object GlobalAgent { get; set; }

        public virtual void OnAwake() { }

        public virtual void OnStart() { }

        public virtual void OnStop() { }

        public virtual void OnEnter() { }
        public virtual void OnExit() { }

        public float LastUpdateTimeStamp { get; private set; }
        public virtual void OnUpdate(float delta) { }
    }

    public class Task<T> : Task
    {
        public T GenericAgent { get; set; }

        public override void OnAwake()
        {
            base.OnAwake();

            if (Agent is T g)
            {
                GenericAgent = g;
            }
            else
            {
                TraceListener.WriteLine($"Agent 类型不匹配");
            }
        }
    }
}

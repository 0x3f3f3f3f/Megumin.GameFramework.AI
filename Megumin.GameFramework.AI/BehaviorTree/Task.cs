using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace Megumin.GameFramework.AI.BehaviorTree
{
    public class Task
    {
        public TraceListener TraceListener { get; set; }

        //public object FileNode { get; set; }
        /// <summary>
        /// 名字不应该是示例的，应该是序列化文件中的，一个BehaviorTree文件可能有多个示例，不要每个示例都存一遍Name?
        /// 存了也没事，string内存中只有一份。
        /// </summary>
        public string Name { get; set; }
        public object Agent { get; set; }
        public static object GlobalAgent { get; set; }

        /// <summary>
        /// On函数接口化
        /// </summary>
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
        public T GenericAgent
        {
            get
            {
                if (Agent is T g)
                {
                    return g;
                }
                return default;
            }
        }


    }

    public enum TaskState
    {
        Running,
        Success,
        Faulture,
    }

    /// <summary>
    /// 后期绑定，动态绑定到Agent的一个符合签名的函数上。
    /// </summary>
    public class LateBindintTask:Task
    {
        Func<float,TaskState> Func { get; set; }

        public override void OnUpdate(float delta)
        {
            base.OnUpdate(delta);
            Func.Invoke(delta);
        }
    }
}

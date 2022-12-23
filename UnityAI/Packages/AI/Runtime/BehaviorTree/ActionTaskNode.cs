using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Megumin.GameFramework.AI.BehaviorTree
{
    /// <summary>
    /// 换个名字，与异步task重名
    /// 
    /// 方法名带不带On,以On开头的方法不应该时public的，不应该由外部类调用。
    /// 
    /// 为了提高性能，成员尽量不要在声明时初始化。
    /// 成员不是在所有情况下都会用到，保持未初始化能有效节省内存。
    /// </summary>
    public class TaskNode
    {
        /// <summary>
        /// tree中唯一ID。子树中可能会出现冲突，使用GUID.ToString 尽量保证唯一性。
        /// </summary>
        string UID;
        /// <summary>
        /// 运行时进程空间唯一ID。
        /// </summary>
        string GUID;

        public bool Enabled { get; internal set; }
        public bool IsStarted { get; internal set; }
        public Status State { get; set; } = Status.Init;

        void Enter()
        {
            Debug.Log($"Enter Node {this.GetType().Name}");
            State = Status.Running;
            OnEnter();


        }

        protected virtual void OnEnter()
        {

        }

        Status Exit(Status result)
        {
            State = OnExit(result);
            Debug.Log($"Exit Node {this.GetType().Name}");
            return State;
        }

        protected virtual Status OnExit(Status result)
        {
            return result;
        }

        /// <summary>
        /// 前置装饰器
        /// </summary>
        public object[] perDerator;

        /// <summary>
        /// <para/> Q:为什么CanEnter不放在Tick内部？
        /// <para/> A:当条件中止时需要在不Tick的状态下获取EnterType，放在Tick里会执行2次CanEnter
        /// </summary>
        /// <returns></returns>
        public EnterType CanEnter()
        {
            if (perDerator?.Length > 0)
            {
                foreach (var pre in perDerator)
                {
                    if (pre is IConditionable conditionable)
                    {
                        if (conditionable.Cal() == false)
                        {
                            return EnterType.False;
                        }
                    }
                }
            }
            return EnterType.True;
        }

        public Status Tick()
        {
            if (State != Status.Running)
            {
                Enter();
            }
            var res = OnTick();
            if (res != Status.Running)
            {
                res = Exit(res);
            }
            return res;
        }

        protected virtual Status OnTick()
        {
            return Status.Succeeded;
        }

        internal void Awake()
        {

        }

        internal void Enable()
        {

        }

        internal void Start()
        {

        }
    }

    internal interface IConditionable
    {
        bool Cal();
        bool Result { get; }
    }

    public class BTTaskNode : TaskNode
    {

    }

    public class CompositeTaskNode : BTTaskNode
    {
        public int current { get; protected set; } = 0;
        public List<TaskNode> children = new List<TaskNode>();

        protected override void OnEnter()
        {
            base.OnEnter();
            current = 0;
        }
    }

    public class OneChildTaskNode : BTTaskNode
    {
        public TaskNode child;
    }

    public class ActionTaskNode : BTTaskNode
    {


    }

    public interface IStartable
    {
        bool IsStarted { get; }
        void Start();
        void Stop();
    }

    public interface IEnableable
    {
        bool Enabled { get; set; }
       
        //void Enable();
        //void Disable();
    }

    /// <summary>
    /// 想要轮询必须支持开启和关闭。这样才能正确处理Start。
    /// </summary>
    public interface ITickable: IEnableable
    {

    }

    public enum Status
    {
        Init,
        Succeeded,
        Failed,
        //Aborted,
        Running,
    };

    [Flags]
    public enum AbortType
    {
        None = 0,
        Self = 1 << 0,
        LowerPriority = 1 << 1,
        Both = Self | LowerPriority
    }

    public enum EnterType
    {
        True,
        False,
        /// <summary>
        /// 有些节点可能调试时临时关闭，需要忽略这些节点。
        /// </summary>
        Ignore,
    }

    /// <summary>
    /// 可层层桥套的计时器，更改父计时器，子计时器也受到影响。
    /// </summary>
    public interface ITimer
    {
        /// <summary>
        /// 重设Scale时，同时重设原点
        /// </summary>
        float Scale { get; }
        float Now { get; }
        float NoScaleNow { get; }
        DateTimeOffset realTimeNow { get; }
        DateTimeOffset Origin { get; }
        /// <summary>
        /// 由父类调用
        /// </summary>
        /// <param name="delta"></param>
        void MoveNext(float delta);
    }

    public class Timer
    {
        public Timer parent;
        double Origin = 0;
        double realtime = Time.realtimeSinceStartupAsDouble;
        double Now => (realtime - Origin) * GlobalScale;
        double localScale;
        double GlobalScale => localScale * (parent?.GlobalScale ?? 1);
    }


}

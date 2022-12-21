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
    /// </summary>
    public class TaskNode
    {
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
        void Enable();
        void Disable();
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
}

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
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Megumin.GameFramework.AI.BehaviorTree
{
    public class TaskNode
    {
        public TaskNodeTickResult Tick()
        {
            return OnTick();
        }

        public virtual TaskNodeTickResult OnTick()
        {
            return TaskNodeTickResult.Succeeded;
        }

        public virtual void OnEnter()
        {

        }
    }

    public enum TaskNodeTickResult
    {
        Succeeded,
        Failed,
        Aborted,
        Running,
    };
}

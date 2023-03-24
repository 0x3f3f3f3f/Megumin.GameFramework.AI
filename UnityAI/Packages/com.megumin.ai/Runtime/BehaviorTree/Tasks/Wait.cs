using Megumin.Binding;
using UnityEngine;

namespace Megumin.GameFramework.AI.BehaviorTree
{
    [Icon("ICONS/sg_graph_icon.png")]
    public class Wait : ActionTaskNode
    {
        public RefVar<float> waitTime = 5.0f;

        float entertime;
        protected override void OnEnter()
        {
            entertime = Time.time;
        }

        protected override Status OnTick(BTNode from)
        {
            //Debug.Log($"Wait Time :{Time.time - entertime}");
            if (Time.time - entertime >= waitTime)
            {
                return Status.Succeeded;
            }
            return Status.Running;
        }
    }
}

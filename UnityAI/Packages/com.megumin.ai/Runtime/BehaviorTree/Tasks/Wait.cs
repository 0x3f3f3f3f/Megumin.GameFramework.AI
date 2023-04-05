using Megumin.Binding;
using UnityEngine;

namespace Megumin.GameFramework.AI.BehaviorTree
{
    [Icon("ICONS/sg_graph_icon.png")]
    public class Wait : BTActionNode, IDetailable
    {
        public RefVar<float> waitTime = 5.0f;

        float entertime;
        private float left;

        protected override void OnEnter()
        {
            entertime = Time.time;
            left = waitTime.Value;
        }

        protected override Status OnTick(BTNode from)
        {
            //Debug.Log($"Wait Time :{Time.time - entertime}");
            left = waitTime.Value - (Time.time - entertime);
            if (left <= 0)
            {
                return Status.Succeeded;
            }
            return Status.Running;
        }

        public string LogString()
        {
            return "Wait: waitTime. Left: 0.5f";
        }

        public string GetDetail()
        {
            if (State == Status.Running)
            {
                return $"Wait: {waitTime.Value:0.000}  Left:{left:0.000}";
            }
            else
            {
                return $"Wait: {waitTime.Value:0.000}";
            }
        }
    }
}

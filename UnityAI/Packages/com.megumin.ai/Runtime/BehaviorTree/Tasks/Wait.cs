using System.ComponentModel;
using Megumin.Binding;
using UnityEngine;

namespace Megumin.GameFramework.AI.BehaviorTree
{
    [Category("Action")]
    [Icon("d_unityeditor.animationwindow@2x")]
    public class Wait : BTActionNode, IDetailable
    {
        public RefVar_Float WaitTime = new() { value = 5.0f };

        float entertime;
        private float left;

        protected override void OnEnter(object options = null)
        {
            entertime = Time.time;
            left = WaitTime;
        }

        protected override Status OnTick(BTNode from, object options = null)
        {
            //Debug.Log($"Wait Time :{Time.time - entertime}");
            left = WaitTime - (Time.time - entertime);
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
                return $"Wait: {(float)WaitTime:0.000}  Left:{left:0.000}";
            }
            else
            {
                return $"Wait: {(float)WaitTime:0.000}";
            }
        }
    }
}

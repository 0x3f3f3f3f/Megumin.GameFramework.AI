using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Megumin.Binding;
using UnityEngine;
using UnityEngine.Serialization;

namespace Megumin.GameFramework.AI.BehaviorTree
{
    [Category("Action")]
    [Icon("console.infoicon@2x")]
    [HelpURL("https://github.com/KumoKyaku/Megumin.GameFramework.AI.Samples/wiki/Log")]
    public class Log : BTActionNode, IDetailable
    {

        public bool LogCount = false;
        public float waitTime = 0.15f;
        public RefVar_String Text = new() { value = "Hello world!" };

        float entertime;
        int count = 0;

        protected override void OnEnter(object options = null)
        {
            entertime = Time.time;
            count++;
        }

        protected override Status OnTick(BTNode from, object options = null)
        {
            if (Time.time - entertime >= waitTime)
            {
                if (LogCount)
                {
                    Debug.Log($"{(string)Text} ---- {count}");
                }
                else
                {
                    Debug.Log((string)Text);
                }

                return Status.Succeeded;
            }
            return Status.Running;
        }

        public string GetDetail()
        {
            return Text;
        }
    }
}

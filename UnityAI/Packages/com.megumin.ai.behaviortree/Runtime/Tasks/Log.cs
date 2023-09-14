using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Megumin.Binding;
using UnityEngine;
using UnityEngine.Serialization;

namespace Megumin.AI.BehaviorTree
{
    [Category("Action")]
    [Icon("console.infoicon@2x")]
    [HelpURL(URL.WikiTask + "Log")]
    public class Log : BTActionNode, IDetailable
    {

        public bool LogCount = false;
        public float waitTime = 0.15f;

        [Obsolete("Use Info.Text instead.")]
        public RefVar_String Text = new() { value = "Hello world!" };
        public LogInfo Info = new LogInfo();

        protected float entertime;
        protected int count = 0;

        protected override void OnEnter(object options = null)
        {
            entertime = Time.time;
            count++;
        }

        protected override Status OnTick(BTNode from, object options = null)
        {
            if (Time.time - entertime >= waitTime)
            {
                LogString();
                return Status.Succeeded;
            }
            return Status.Running;
        }


        public virtual void LogString()
        {
            var sb = Info.Rebuid();

            if (LogCount)
            {
                sb.Append("----");
                sb.Append(count.ToString());
            }

            Debug.Log(sb.ToString());
        }

        public virtual string GetDetail()
        {
            return Info.Text;
        }
    }
}

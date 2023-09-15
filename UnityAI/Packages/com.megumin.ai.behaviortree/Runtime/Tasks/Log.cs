using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Megumin.Binding;
using Megumin.Timers;
using UnityEngine;
using UnityEngine.Serialization;

namespace Megumin.AI.BehaviorTree
{
    [Category("Action")]
    [Icon("console.infoicon@2x")]
    [HelpURL(URL.WikiTask + "Log")]
    public class Log : BTActionNode, IDetailable
    {
        [Space]
        [Tooltip("Use my gameObject  macro replace Text.")]
        public bool UseMacro = false;
        public bool LogCount = false;

        [Space]
        public float waitTime = 0.15f;

        public LogInfo Info = new LogInfo();

        protected IWaitTimeable<double> waitTimeable { get; } = new WaitGameTime();

        protected int count = 0;
        protected override void OnEnter(object options = null)
        {
            waitTimeable.WaitStart();
            count++;
        }

        protected override Status OnTick(BTNode from, object options = null)
        {
            if (waitTimeable.WaitEnd(waitTime))
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

            if (UseMacro)
            {
                sb = sb.MacroUnityObject(GameObject);
            }

            Debug.Log(sb.ToString());
        }

        public virtual string GetDetail()
        {
            return Info.Text;
        }
    }
}

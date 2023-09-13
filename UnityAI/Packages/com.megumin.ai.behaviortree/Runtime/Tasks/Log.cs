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
        public RefVar_String Text = new() { value = "Hello world!" };

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

        StringBuilder StringBuilder = new StringBuilder();
        public virtual void LogString()
        {
            StringBuilder.Clear();
            StringBuilder.Append(Text);

            if (LogCount)
            {
                StringBuilder.Append("----");
                StringBuilder.Append(count.ToString());
            }

            Debug.Log(StringBuilder.ToString());
        }

        public virtual string GetDetail()
        {
            return Text;
        }
    }


    [Category("Action")]
    [Icon("console.infoicon@2x")]
    [HelpURL(URL.WikiTask + "Log")]
    public class Log2 : Log
    {
        public RefVar_Transform Ref_Transform;
        public RefVar_GameObject Ref_GameObject;

        StringBuilder StringBuilder = new StringBuilder();
        public override void LogString()
        {
            StringBuilder.Clear();
            StringBuilder.Append(Text);
            if (Ref_Transform?.Value)
            {
                StringBuilder.Append("----");
                StringBuilder.Append(Ref_Transform.Value.name);
            }

            if (Ref_GameObject?.Value)
            {
                StringBuilder.Append("----");
                StringBuilder.Append(Ref_GameObject.Value.name);
            }

            if (LogCount)
            {
                StringBuilder.Append("----");
                StringBuilder.Append(count.ToString());
            }

            Debug.Log(StringBuilder.ToString());
        }

    }
}

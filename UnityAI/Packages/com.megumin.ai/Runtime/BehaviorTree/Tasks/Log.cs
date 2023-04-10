using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Megumin.Binding;
using UnityEngine;

namespace Megumin.GameFramework.AI.BehaviorTree
{
    [Category("Debug")]
    [Icon("Icons/Overlays/ToolsToggle.png")]
    [HelpURL("www.baidu.com")]
    [Description]
    [Tooltip("在控制台打印日志")]
    public class Log : BTActionNode, IDetailable
    {
        int count = 0;
        public float waitTime = 0.15f;
        public string LogStr = "Hello world!";
        public RefVar<string> LogStr2;
        float entertime;

        protected override void OnEnter()
        {
            entertime = Time.time;
            count++;
        }

        protected override Status OnTick(BTNode from)
        {
            if (Time.time - entertime >= waitTime)
            {
                Debug.Log($"LogStr:{LogStr} ---- LogStr2:{LogStr2?.Value} ---- {count}");
                return Status.Succeeded;
            }
            return Status.Running;
        }

        public string GetDetail()
        {
            return $"LogStr:{LogStr} ---- LogStr2:{LogStr2?.Value}";
        }
    }
}

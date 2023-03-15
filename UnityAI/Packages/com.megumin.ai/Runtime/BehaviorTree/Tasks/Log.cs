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
    public class Log : ActionTaskNode
    {
        int count = 0;

        public string LogStr = "Hello world!";
        public RefVariable<string> LogStr2;

        protected override void OnEnter()
        {
            count++;
        }

        protected override Status OnTick()
        {
            Debug.Log($"LogStr:{LogStr} ---- LogStr2:{LogStr2.Value} ---- {count}");
            return Status.Succeeded;
        }
    }
}

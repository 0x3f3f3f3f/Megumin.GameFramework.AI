using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Megumin.GameFramework.AI.BehaviorTree
{
    [Category("Debug/ActionTaskNode")]
    [Icon("Icons/Overlays/ToolsToggle.png")]
    [HelpURL("www.baidu.com")]
    [Description]
    [Tooltip("在控制台打印日志")]
    public class TestActionNode : ActionTaskNode
    {
    }

    [Category("Debug/BTDecorator")]
    [Icon("Icons/Overlays/ToolsToggle.png")]
    [HelpURL("www.baidu.com")]
    [Description]
    [Tooltip("在控制台打印日志")]
    public class TestBTDecorator: BTDecorator
    {

    }
}

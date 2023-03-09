using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Megumin.GameFramework.AI.BehaviorTree
{
    [Category("Samples/Attribute")]
    [Icon("Icons/Overlays/ToolsToggle.png")]
    [HelpURL("www.github.com")]
    [Tooltip("在控制台打印日志")]
    [Description("TestActionNode")]
    [Color(0.82f, 0.58f, 0.23f, 0.75f)]
    public class TestActionNode : ActionTaskNode
    {
    }

    [Category("Samples/Attribute")]
    [Icon("Icons/Overlays/ToolsToggle.png")]
    [HelpURL("www.baidu.com")]
    [Description("TestDescription")]
    [Tooltip("在控制台打印日志")]
    [Color(0.72f, 0.25f, 0.27f)]
    public class TestBTDecorator : BTDecorator
    {

    }


}

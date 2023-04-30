using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

namespace Megumin.GameFramework.AI.BehaviorTree
{
    [DisplayName("DisplayName_TestActionNode")]
    [Category("Samples/Attribute")]
    [Icon("Icons/Overlays/ToolsToggle.png")]
    [HelpURL("www.github.com")]
    [Tooltip("在控制台打印日志")]
    [Description("TestActionNode")]
    [Color(0.82f, 0.58f, 0.23f, 0.75f)]
    public class TestActionNode : BTActionNode, IAbortable, IBuildContextualMenuable
    {
        [field: SerializeField]
        public AbortType AbortType { get; set; } = AbortType.LowerPriority;

        public void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            evt.menu.AppendAction("NodeCustomMenu", a => { Debug.Log(111); }, DropdownMenuAction.Status.Normal);
        }
    }

    [DisplayName("DisplayName_TestBTDecorator")]
    [Category("Samples/Attribute")]
    [Icon("Icons/Overlays/ToolsToggle.png")]
    [HelpURL("www.baidu.com")]
    [Description("TestDescription")]
    [Tooltip("在控制台打印日志")]
    [Color(0.72f, 0.25f, 0.27f)]
    public class TestBTDecorator : BTDecorator, ITitleable
    {
        public string Title => "ITitleable_MyTitle";
    }


}

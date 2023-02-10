using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UIElements;

namespace Megumin.GameFramework.AI.BehaviorTree.Editor
{
    public partial class BehaviorTreeNodeView
    {
        private void BuildContextualMenuDecorator(ContextualMenuPopulateEvent evt)
        {
            evt.menu.AppendAction("AddCheckBool", a => AddCheckBool(), DropdownMenuAction.AlwaysEnabled);
            evt.menu.AppendSeparator();
        }

        private void AddCheckBool()
        {
            var check = new CheckBool();
            SONode.Node.AddDecorator(check);
        }
    }
}

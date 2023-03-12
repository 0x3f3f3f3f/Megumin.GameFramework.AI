using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UIElements;

namespace Megumin.GameFramework.AI.BehaviorTree.Editor
{
    internal static class BehaviorTreeEditorExtension
    {
        public static void AppendActionTODO(this DropdownMenu menu,
                                            string actionName,
                                            Action<DropdownMenuAction> action,
                                            Func<DropdownMenuAction, DropdownMenuAction.Status> actionStatusCallback,
                                            object userData = null)
        {

            menu.AppendAction($"TODO: {actionName}",
                a =>
                {
                    throw new NotImplementedException();
                },
                BehaviorTreeEditor.TODO);
        }

        public static void AppendActionTODO(this DropdownMenu menu,
                                            string actionName,
                                            Action<DropdownMenuAction> action,
                                            DropdownMenuAction.Status status = DropdownMenuAction.Status.Normal)
        {
            menu.AppendAction($"TODO: {actionName}",
                a =>
                {
                    throw new NotImplementedException();
                },
                BehaviorTreeEditor.TODO);
        }
    }
}





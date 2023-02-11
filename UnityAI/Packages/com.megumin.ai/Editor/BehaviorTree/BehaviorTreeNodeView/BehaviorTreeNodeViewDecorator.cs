using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TreeEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Megumin.GameFramework.AI.BehaviorTree.Editor
{
    public partial class BehaviorTreeNodeView
    {
        private void BuildContextualMenuDecorator(ContextualMenuPopulateEvent evt)
        {
            evt.menu.AppendAction("AddCheckBool", a => AddCheckBool(), DropdownMenuAction.AlwaysEnabled);
            evt.menu.AppendAction("Add Decorator", a => OpenDecoratorSearchWindow(a), DropdownMenuAction.AlwaysEnabled);

            for (int i = nearDType.Count - 1; i >= 0; i--)
            {
                var type = nearDType[i];
                evt.menu.AppendAction($"Add Decorator/{type.Name}", a => AddDecorator(type), DropdownMenuAction.AlwaysEnabled);
            }

            evt.menu.AppendSeparator();
        }

        private void OpenDecoratorSearchWindow(DropdownMenuAction a)
        {
            CreateDecoratorSearchWindowProvider decoratorSearchWindowProvider
            = ScriptableObject.CreateInstance<CreateDecoratorSearchWindowProvider>();
            decoratorSearchWindowProvider.hideFlags = HideFlags.DontSave;
            decoratorSearchWindowProvider.NodeView = this;

            var screenMousePosition = TreeView.EditorWindow.position.position + a.eventInfo.mousePosition;
            SearchWindow.Open(new SearchWindowContext(screenMousePosition), decoratorSearchWindowProvider);
        }

        private void AddCheckBool()
        {
            var check = new CheckBool();
            SONode.Node.AddDecorator(check);
        }

        /// <summary>
        /// 最近常用的装饰器
        /// </summary>
        static List<Type> nearDType = new List<Type>();
        internal object AddDecorator(Type type)
        {
            TreeView.UndoRecord($"AddDecorator  [{type.Name}]");
            this.LogMethodName();
            var decorator = Activator.CreateInstance(type);
            SONode.Node.AddDecorator(decorator);

            //去重添加
            nearDType.Remove(type);
            nearDType.Add(type);

            return decorator;
        }
    }
}

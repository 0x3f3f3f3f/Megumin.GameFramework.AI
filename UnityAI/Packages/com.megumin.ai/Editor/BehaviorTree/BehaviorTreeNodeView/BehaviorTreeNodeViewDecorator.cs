using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            evt.menu.AppendAction("Add Decorator", a => OpenDecoratorSearchWindow(), DropdownMenuAction.AlwaysEnabled);
            evt.menu.AppendAction("Add Decorator Fast", a => OpenDecoratorSearchWindow(), DropdownMenuAction.AlwaysEnabled);
            evt.menu.AppendSeparator();
        }

        private void OpenDecoratorSearchWindow()
        {
            CreateDecoratorSearchWindowProvider decoratorSearchWindowProvider
            = ScriptableObject.CreateInstance<CreateDecoratorSearchWindowProvider>();
            decoratorSearchWindowProvider.hideFlags= HideFlags.DontSave;
            decoratorSearchWindowProvider.NodeView = this;
            SearchWindow.Open(new SearchWindowContext(TreeView.LastContextualMenuMousePosition), decoratorSearchWindowProvider);
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
            this.LogMethodName();
            var decorator = Activator.CreateInstance(type);
            SONode.Node.AddDecorator(decorator);
            nearDType.Add(type);
            return decorator;
        }
    }
}

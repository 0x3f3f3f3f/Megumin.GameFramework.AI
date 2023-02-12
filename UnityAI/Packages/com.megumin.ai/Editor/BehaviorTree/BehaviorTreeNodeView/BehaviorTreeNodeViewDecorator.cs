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
            evt.menu.AppendAction("Add Decorator", a => OpenDecoratorSearchWindow(a), DropdownMenuAction.AlwaysEnabled);

            for (int i = nearDType.Count - 1; i >= 0; i--)
            {
                var type = nearDType[i];
                evt.menu.AppendAction($"Add Decorator/{type.Name}", a => AddDecorator(type), DropdownMenuAction.AlwaysEnabled);
            }

            this.LogMethodName(evt.ToStringReflection(), "\n", evt.triggerEvent.ToStringReflection());

            if (evt.currentTarget is BehaviorTreeDecoratorView decoratorView)
            {
                evt.menu.AppendAction($"Remove Decorator", a => RemoveDecorator(decoratorView), DropdownMenuAction.AlwaysEnabled);
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

            //刷新UI
            DecoretorListView.Rebuild();
            return decorator;
        }

        public void CreateDecoratorView(BTNode node)
        {
            if (node.Decorators != null)
            {
                DecoretorListView.itemsSource = node.Decorators;
                DecoretorListView.Rebuild();
            }
        }
        private void RemoveDecorator(BehaviorTreeDecoratorView decoratorView)
        {
            this.LogMethodName(decoratorView);
        }

        internal protected VisualElement ListViewMakeDecoratorView()
        {
            var elem = new BehaviorTreeDecoratorView();
            return elem;
        }

        internal protected void ListViewBindDecorator(VisualElement view, int index)
        {
            if (view is BehaviorTreeDecoratorView decoratorView)
            {
                var decorator = SONode.Node.Decorators[index];
                decoratorView.SetDecorator(decorator);
            }
        }
    }
}

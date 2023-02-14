using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TreeEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;

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
            RefreshDecoratorListView();

            return decorator;
        }

        public void RefreshDecoratorListView()
        {
            DecoretorListView.itemsSource = SONode.Node.Decorators;
            DecoretorListView.Rebuild();
        }

        internal void RemoveDecorator(BehaviorTreeDecoratorView decoratorView)
        {
            this.LogMethodName(decoratorView);
            TreeView.UndoRecord($"decoratorView  [{decoratorView.Decorator.GetType().Name}]");
            if (SONode.Node.Decorators != null)
            {
                SONode.Node.RemoveDecorator(decoratorView.Decorator);
            }

            //刷新UI
            RefreshDecoratorListView();
        }

        internal protected VisualElement ListViewMakeDecoratorView()
        {
            var elem = new BehaviorTreeDecoratorView();
            elem.NodeView = this;
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

        private void DecoretorListView_onItemsChosen(IEnumerable<object> obj)
        {
            this.LogMethodName(obj.FirstOrDefault());
        }

        private void DecoretorListView_itemIndexChanged(int arg1, int arg2)
        {
            this.LogMethodName(arg1, arg2);
        }

        internal void MoveUpDecorator(BehaviorTreeDecoratorView decoratorView)
        {


            if (SONode.Node.Decorators != null)
            {
                for (int i = 0; i < SONode.Node.Decorators.Length; i++)
                {
                    var d = SONode.Node.Decorators[i];
                    if (d == decoratorView.Decorator)
                    {
                        if (i != 0)
                        {
                            TreeView.UndoRecord($"MoveUpDecorator  [{decoratorView.Decorator.GetType().Name}]  {i} -> {i - 1}");
                            var prev = SONode.Node.Decorators[i - 1];
                            SONode.Node.Decorators[i - 1] = d;
                            SONode.Node.Decorators[i] = prev;
                            RefreshDecoratorListView();
                        }
                    }
                }
            }
        }

        internal void MoveDownDecorator(BehaviorTreeDecoratorView decoratorView)
        {
            if (SONode.Node.Decorators != null)
            {
                for (int i = 0; i < SONode.Node.Decorators.Length; i++)
                {
                    var d = SONode.Node.Decorators[i];
                    if (d == decoratorView.Decorator)
                    {
                        if (i != SONode.Node.Decorators.Length - 1)
                        {
                            TreeView.UndoRecord($"MoveDownDecorator  [{decoratorView.Decorator.GetType().Name}]  {i} -> {i - 1}");
                            var next = SONode.Node.Decorators[i + 1];
                            SONode.Node.Decorators[i + 1] = d;
                            SONode.Node.Decorators[i] = next;

                            RefreshDecoratorListView();
                        }
                    }
                }
            }
        }
    }
}

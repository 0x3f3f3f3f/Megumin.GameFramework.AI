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

        internal void DecoretorListView_itemIndexChanged(int currrent, int des)
        {
            this.LogMethodName(currrent, des);
            var list = SONode.Node.Decorators;
            if (list != null && currrent >= 0 && currrent < list.Count && des >= 0 && des < list.Count)
            {
                var target = list[currrent];
                TreeView.UndoRecord($"Move Decorator  [{target.GetType().Name}]  {currrent} -> {des}");
                list.Remove(target);
                list.Insert(des, target);
                RefreshDecoratorListView();
            }
        }

        internal void MoveUpDecorator(BehaviorTreeDecoratorView decoratorView)
        {
            var list = SONode.Node.Decorators;
            if (list != null)
            {
                var index = list.IndexOf(decoratorView.Decorator);
                DecoretorListView_itemIndexChanged(index, index - 1);
            }
        }

        internal void MoveDownDecorator(BehaviorTreeDecoratorView decoratorView)
        {
            var list = SONode.Node.Decorators;
            if (list != null)
            {
                var index = list.IndexOf(decoratorView.Decorator);
                DecoretorListView_itemIndexChanged(index, index + 1);
            }
        }
    }
}

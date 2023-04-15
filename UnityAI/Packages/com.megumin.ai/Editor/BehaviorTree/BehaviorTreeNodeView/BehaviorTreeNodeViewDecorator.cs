using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Megumin.GameFramework.AI.BehaviorTree.Editor
{
    public partial class BehaviorTreeNodeView
    {
        private void BuildContextualMenuDecorator(ContextualMenuPopulateEvent evt)
        {
            evt.menu.AppendAction("Add Decorator", a => OpenDecoratorSearchWindow(a), DropdownMenuAction.Status.Normal);

            for (int i = nearDType.Count - 1; i >= 0; i--)
            {
                var type = nearDType[i];
                evt.menu.AppendAction($"Add Decorator/{type.Name}", a => AddDecorator(type), DropdownMenuAction.Status.Normal);
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

            if (BehaviorTreeEditor.EditorLog)
            {
                this.LogMethodName();
            }

            var decorator = SONode.Node.AddDecorator(type);
            //去重添加
            nearDType.Remove(type);
            nearDType.Add(type);

            //刷新UI
            RefreshDecoratorListView();

            return decorator;
        }

        public void RefreshDecoratorListView()
        {
            DecoretorListView.itemsSource = Node?.Decorators;
            DecoretorListView.Rebuild();
            if (DecoretorListView.itemsSource == null || DecoretorListView.itemsSource.Count == 0)
            {
                decoratorContainer.AddToClassList("unDisplay");
            }
            else
            {
                decoratorContainer.RemoveFromClassList("unDisplay");
            }
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

        List<BehaviorTreeDecoratorView> AllDecoratorView = new();
        internal protected VisualElement ListViewMakeDecoratorView()
        {
            var elem = new BehaviorTreeDecoratorView();
            elem.NodeView = this;
            AllDecoratorView.Add(elem);
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

        internal protected void DecoretorListView_destroyItem(VisualElement obj)
        {
            if (obj is BehaviorTreeDecoratorView decoratorView)
            {
                AllDecoratorView.Remove(decoratorView);
            }
        }

        private void DecoretorListView_onItemsChosen(IEnumerable<object> obj)
        {
            if (BehaviorTreeEditor.EditorLog)
            {
                this.LogMethodName(obj.FirstOrDefault());
            }
        }

        internal void DecoretorListView_itemIndexChanged(int currrent, int des)
        {
            if (BehaviorTreeEditor.EditorLog)
            {
                this.LogMethodName(currrent, des);
            }

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

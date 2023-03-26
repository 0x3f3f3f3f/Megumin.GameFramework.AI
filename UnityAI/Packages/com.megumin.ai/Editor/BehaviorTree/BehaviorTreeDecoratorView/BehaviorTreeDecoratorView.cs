using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Megumin.GameFramework.AI.Editor;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Megumin.GameFramework.AI.BehaviorTree.Editor
{
    public partial class BehaviorTreeDecoratorView : GraphElement
    {
        public override VisualElement contentContainer => ContentContainer;
        public VisualElement ContentContainer { get; private set; }
        public Button Icon { get; private set; }
        public Button AbortTypeButton { get; private set; }
        public Label Title { get; }
        public VisualElement CMarker { get; private set; }
        public VisualElement FMarker { get; private set; }
        public VisualElement BMarker { get; private set; }
        public VisualElement AMarker { get; private set; }

        public new class UxmlFactory : UxmlFactory<BehaviorTreeDecoratorView, UxmlTraits> { }

        public BehaviorTreeDecoratorView()
        {
            var visualTree = Resources.Load<VisualTreeAsset>("BehaviorTreeDecoratorView");
            visualTree.CloneTree(this);

            ContentContainer = this.Q("contentContainer");
            Icon = this.Q<Button>("icon", "treeElementIcon");
            AbortTypeButton = this.Q<Button>("abortType", "abortType");
            Title = this.Q<Label>("title-label");
            CMarker = this.Q("cMarker");
            FMarker = this.Q("fMarker");
            BMarker = this.Q("bMarker");
            AMarker = this.Q("aMarker");

            //this.AddManipulator(new TestMouseManipulator());
            //pickingMode = PickingMode.Position;
            this.AddManipulator(new ContextualMenuManipulator(BuildContextualMenu));
            this.AddManipulator(new DoubleClickSelector(OnDoubleClick));
            capabilities |= Capabilities.Selectable | Capabilities.Deletable | Capabilities.Ascendable | Capabilities.Copiable | Capabilities.Snappable | Capabilities.Groupable;
            usageHints = UsageHints.DynamicTransform;
            AddToClassList("decorator");

            this.RegisterCallback<TooltipEvent>(OnToolTip);
        }

        private void OnDoubleClick(MouseDownEvent evt)
        {
            this.LogMethodName();
        }

        public void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            //this.LogMethodName(evt.ToStringReflection(), "\n", evt.triggerEvent.ToStringReflection());
            if (evt.target is BehaviorTreeDecoratorView)
            {
                evt.menu.AppendAction("Open Documentation _F1", a => AI.Editor.Utility.OpenDocumentation(Decorator?.GetType()),
                     Decorator?.GetType().TryGetAttribute<HelpURLAttribute>(out var _) ?? false ?
                     DropdownMenuAction.Status.Normal : DropdownMenuAction.Status.Disabled);

                evt.menu.AppendSeparator();

                evt.menu.AppendActionTODO("Breakpoint", a => { }, DropdownMenuAction.Status.Normal);
                evt.menu.AppendSeparator();
            }

            evt.menu.AppendAction($"Delete", a => NodeView?.RemoveDecorator(this), DropdownMenuAction.Status.Normal);
            evt.menu.AppendSeparator();

            //拖拽Bug有点多，暂时用菜单实现。
            evt.menu.AppendAction($"Move Up", a => NodeView?.MoveUpDecorator(this), DropdownMenuAction.Status.Normal);
            evt.menu.AppendAction($"Move Down", a => NodeView?.MoveDownDecorator(this), DropdownMenuAction.Status.Normal);
            evt.menu.AppendSeparator();

            evt.menu.AppendAction("Open Decorator Script", a => AI.Editor.Utility.OpenScript(Decorator?.GetType()), DropdownMenuAction.Status.Normal);
            evt.menu.AppendActionTODO("Open Decorator View Script", a => { }, DropdownMenuAction.Status.Normal);
            evt.menu.AppendAction("Select Decorator Script", a => AI.Editor.Utility.SelectScript(Decorator?.GetType()), DropdownMenuAction.Status.Normal);
            evt.menu.AppendSeparator();
        }

        public BehaviorTreeNodeView NodeView { get; internal set; }
        public ITreeElement Decorator { get; private set; }
        public DecoratorWrapper SODecorator;

        internal void SetDecorator(ITreeElement decorator)
        {
            this.Decorator = decorator;
            ReloadView();
        }

        public DecoratorWrapper CreateSOWrapperIfNull(ITreeElement decorator, bool forceRecreate = false)
        {
            if (decorator == null)
            {
                return SODecorator;
            }

            var soWrapper = SODecorator;
            if (!soWrapper)
            {
                if (NodeView.TreeView.DecoratorWrapperCache.TryGetValue(decorator.GUID, out var cacheWrapper))
                {
                    //创建新的SO对象在 Inpector锁定显示某个节点时，会出现无法更新的问题。
                    //尝试复用旧的SOWrapper

                    //Debug.Log("尝试复用旧的SOWrapper");
                    soWrapper = cacheWrapper;
                }
            }

            if (!soWrapper || forceRecreate)
            {
                soWrapper = this.CreateSOWrapper<DecoratorWrapper>();
                NodeView.TreeView.DecoratorWrapperCache[decorator.GUID] = soWrapper;
            }
            return soWrapper;
        }


        public void ReloadView()
        {
            SODecorator = CreateSOWrapperIfNull(Decorator);
            SODecorator.View = this;
            SODecorator.Decorator = Decorator;
            var type = Decorator?.GetType();
            Title.text = Decorator.GetTitle();

            //使用自定义图标
            Icon.TrySetIconFromAttribute(type);

            RefreshAbortTypeUI();

            CMarker.SetToClassList(UssClassConst.enableMarker, Decorator is IConditionDecorator);
            FMarker.SetToClassList(UssClassConst.enableMarker, Decorator is IPreDecorator);
            BMarker.SetToClassList(UssClassConst.enableMarker, Decorator is IPostDecorator);
            AMarker.SetToClassList(UssClassConst.enableMarker, Decorator is IAbortDecorator);


            var attri = type?.GetCustomAttribute<ColorAttribute>();
            if (attri != null)
            {
                contentContainer.style.backgroundColor = attri.Color;
            }
        }

        public void RefreshAbortTypeUI()
        {
            var hasSelf = (Decorator as IAbortable)?.AbortType.HasFlag(AbortType.Self) ?? false;
            AbortTypeButton.SetToClassList(UssClassConst.abortTypeSelf, hasSelf);
            var hasLow = (Decorator as IAbortable)?.AbortType.HasFlag(AbortType.LowerPriority) ?? false;
            AbortTypeButton.SetToClassList(UssClassConst.abortTypeLowerPriority, hasLow);
            AbortTypeButton.tooltip = null;
            if (hasSelf && hasLow)
            {
                AbortTypeButton.tooltip = "AbortType.Both";
            }
            else if (hasSelf)
            {
                AbortTypeButton.tooltip = "AbortType.Self";
            }
            else if (hasLow)
            {
                AbortTypeButton.tooltip = "AbortType.LowerPriority";
            }
        }

        public override void OnSelected()
        {
            //this.LogMethodName(this);
            base.OnSelected();

            if (SODecorator)
            {
                Selection.activeObject = SODecorator;
            }
        }

        protected void OnToolTip(TooltipEvent evt)
        {
            evt.rect = this.worldBound;

            if (Decorator.TryGetToolTipString(out var tooltip))
            {
                evt.tooltip = tooltip;
            }

            evt.StopPropagation();
        }

        //protected override void ExecuteDefaultActionAtTarget(EventBase evt)
        //{
        //    base.ExecuteDefaultActionAtTarget(evt);

        //    if (evt.eventTypeId == EventBase<MouseDownEvent>.TypeId())
        //    {
        //        this.LogMethodName(evt.ToStringReflection());
        //    }
        //}

        //protected override void ExecuteDefaultAction(EventBase evt)
        //{

        //    base.ExecuteDefaultAction(evt);

        //    if (evt.eventTypeId == EventBase<MouseEnterEvent>.TypeId())
        //    {
        //        this.LogMethodName(evt.ToStringReflection());
        //    }
        //}
    }
}

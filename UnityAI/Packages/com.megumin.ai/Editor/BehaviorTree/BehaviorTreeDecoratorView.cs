using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

namespace Megumin.GameFramework.AI.BehaviorTree.Editor
{
    public class BehaviorTreeDecoratorView : VisualElement
    {
        public Label Title { get; }

        public new class UxmlFactory : UxmlFactory<BehaviorTreeDecoratorView, UxmlTraits> { }

        public BehaviorTreeDecoratorView()
        {
            var visualTree = Resources.Load<VisualTreeAsset>("BehaviorTreeDecoratorView");
            visualTree.CloneTree(this);

            Title = this.Q<Label>("title-label");
            //this.AddManipulator(new TestMouseManipulator());
            //pickingMode = PickingMode.Position;
            this.AddManipulator(new ContextualMenuManipulator(BuildContextualMenu));
        }

        public void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            evt.menu.AppendAction($"Remove Decorator", a => NodeView?.RemoveDecorator(this), DropdownMenuAction.AlwaysEnabled);
            //evt.StopPropagation();
        }

        public override VisualElement contentContainer => base.contentContainer;

        public BehaviorTreeNodeView NodeView { get; internal set; }
        public object Decorator { get; private set; }

        internal void SetDecorator(object decorator)
        {
            Title.text = decorator?.GetType().Name;
            this.Decorator = decorator;
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

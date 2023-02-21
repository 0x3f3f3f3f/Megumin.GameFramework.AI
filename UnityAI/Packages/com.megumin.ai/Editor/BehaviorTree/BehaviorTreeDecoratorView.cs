﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Megumin.GameFramework.AI.Editor;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Megumin.GameFramework.AI.BehaviorTree.Editor
{
    public class BehaviorTreeDecoratorView : GraphElement
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
            this.AddManipulator(new DoubleClickSelector(OnDoubleClick));
            capabilities |= Capabilities.Selectable | Capabilities.Deletable | Capabilities.Ascendable | Capabilities.Copiable | Capabilities.Snappable | Capabilities.Groupable;
            usageHints = UsageHints.DynamicTransform;
            AddToClassList("decorator");
        }

        private void OnDoubleClick(MouseDownEvent evt)
        {
            this.LogMethodName();
        }

        public void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            //this.LogMethodName(evt.ToStringReflection(), "\n", evt.triggerEvent.ToStringReflection());

            evt.menu.AppendAction($"Remove Decorator", a => NodeView?.RemoveDecorator(this), DropdownMenuAction.AlwaysEnabled);
            evt.menu.AppendSeparator();

            //拖拽Bug有点多，暂时用菜单实现。
            evt.menu.AppendAction($"Move Up", a => NodeView?.MoveUpDecorator(this), DropdownMenuAction.AlwaysEnabled);
            evt.menu.AppendAction($"Move Down", a => NodeView?.MoveDownDecorator(this), DropdownMenuAction.AlwaysEnabled);
            evt.menu.AppendSeparator();

            evt.menu.AppendAction("Open Decorator Script", a => OpenDecoratorScript(), DropdownMenuAction.AlwaysEnabled);
            evt.menu.AppendAction("Open Decorator View Script", a => OpenDecoratorViewScript(), DropdownMenuAction.AlwaysDisabled);
            evt.menu.AppendSeparator();
        }

        private void OpenDecoratorViewScript()
        {
            throw new NotImplementedException();
        }

        private void OpenDecoratorScript()
        {
            //Todo Cache /unity background tasks
            var scriptGUIDs = AssetDatabase.FindAssets($"t:script");

            foreach (var scriptGUID in scriptGUIDs)
            {
                var assetPath = AssetDatabase.GUIDToAssetPath(scriptGUID);
                var script = AssetDatabase.LoadAssetAtPath<MonoScript>(assetPath);
                var type = Decorator.GetType();
                var code = script.text;
                if (code.Contains($"class {type?.Name}")
                    && code.Contains(type?.Namespace))
                {
                    AssetDatabase.OpenAsset(script, 0, 0);
                }
            }
        }

        public override VisualElement contentContainer => base.contentContainer;

        public BehaviorTreeNodeView NodeView { get; internal set; }
        public object Decorator { get; private set; }
        public DecoratorWrapper SODecorator;

        internal void SetDecorator(object decorator)
        {
            Title.text = decorator?.GetType().Name;
            this.Decorator = decorator;

            if (!SODecorator)
            {
                SODecorator = this.CreateSOWrapper<DecoratorWrapper>();
                SODecorator.Decorator = decorator;
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

    public class DecoratorWrapper : ScriptableObject
    {
        [SerializeReference]
        public object Decorator;



        [Editor]
        public void Test()
        {

        }
    }
}

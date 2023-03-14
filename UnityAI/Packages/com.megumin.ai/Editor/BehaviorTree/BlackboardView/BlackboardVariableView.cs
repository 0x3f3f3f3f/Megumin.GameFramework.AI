using System;
using Megumin.Binding;
using Megumin.GameFramework.AI.Editor;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UIElements;

namespace Megumin.GameFramework.AI.BehaviorTree.Editor
{
    public class BlackboardVariableView : GraphElement
    {
        public new class UxmlFactory : UxmlFactory<BlackboardVariableView, UxmlTraits> { }

        public BlackboardField BlackboardField { get; private set; }
        public TextField m_TextField { get; }
        public VisualElement Body { get; private set; }
        public BlackboardRow BlackboardRow { get; private set; }
        public BehaviorTreeBlackboardView Blackboard { get; internal set; }

        public BlackboardVariableView()
        {
            BlackboardField = new BlackboardField() { text = "Variable", typeText = "" };
            BlackboardField.capabilities = Capabilities.Selectable | Capabilities.Droppable | Capabilities.Renamable;

            {
                //Copy form Unity C# reference source
                m_TextField = BlackboardField.Q<TextField>("textField");
                Assert.IsTrue(m_TextField != null);

                m_TextField.style.display = DisplayStyle.None;

                var textinput = m_TextField.Q(TextField.textInputUssName);
                //TrickleDown.NoTrickleDown 保证我们的回调先执行。
                textinput.RegisterCallback<FocusOutEvent>(e => { OnEditTextFinished(); }, TrickleDown.NoTrickleDown);
            }

            Body = new VisualElement() { name = "body" };
            BlackboardRow = new BlackboardRow(BlackboardField, Body);

            //this.AddManipulator(new ContextualMenuManipulator(BuildContextualMenu));
            //https://forum.unity.com/threads/is-there-an-easy-way-to-customize-the-context-menu-of-a-graphviews-blackboardfield.1006442/
            this.RegisterCallback<ContextualMenuPopulateEvent>(BuildContextualMenu);

            capabilities |= Capabilities.Selectable | Capabilities.Deletable | Capabilities.Ascendable | Capabilities.Copiable | Capabilities.Snappable | Capabilities.Groupable;
            this.Add(BlackboardRow);
        }

        private void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            evt.menu.AppendAction($"Delete", a => { OnDelete(); }, DropdownMenuAction.Status.Normal);
            evt.menu.AppendAction($"Test", a => { DebugTest(); }, DropdownMenuAction.Status.Normal);
        }

        private void DebugTest()
        {
            var a = BlackboardField.IsSelectable();
            var b = BlackboardRow.IsSelectable();
            var c = this.IsSelectable();
        }

        private void OnDelete()
        {
            Blackboard?.RemoveVariable(this);
        }

        private void OnEditTextFinished()
        {
            var table = Blackboard?.LookupTable;
            if (table != null)
            {
                if (m_TextField.text == Variable.RefName)
                {
                    //值没有变，不要验证名字。否则会在原名字上 + （1）
                    return;
                }

                string name = table.ValidName(m_TextField.text);
                if (name != m_TextField.text)
                {
                    m_TextField.SetValueWithoutNotify(name);
                }
            }

            var newName = m_TextField.text;
            Blackboard?.TreeView?.UndoRecord("Change Variable Name");
            Variable.RefName = newName;
            BlackboardField.text = newName;
        }

        public IRefable Variable { get; private set; }
        public void SetVariable(IRefable instance)
        {
            Variable = instance;
            Body.Clear();

            if (Variable == null)
            {
                BlackboardField.text = "MissVariable";
                BlackboardField.typeText = "";
                return;
            }

            if (Guid.TryParse(instance.RefName, out var guid))
            {
                //使用短名字
                BlackboardField.text = instance.RefName;
            }
            else
            {
                BlackboardField.text = instance.RefName;
            }

            //BlackboardField.tooltip = instance.RefName;
            Type type = null;
            if (instance is IVariable mMDataable)
            {
                type = mMDataable.GetValue()?.GetType();
            }

            //var type = instance?.GetValue()?.GetType();
            if (type != null)
            {
                BlackboardField.typeText = type.Name;
            }
            else
            {
                var instanceType = instance.GetType();
                if (instanceType.IsGenericType)
                {
                    BlackboardField.typeText = instanceType.GetGenericArguments()?[0]?.Name;
                }
                else
                {
                    foreach (var item in instanceType.GetInterfaces())
                    {
                        if (item.IsGenericType && item.GetGenericTypeDefinition() == typeof(IVariable<>))
                        {
                            BlackboardField.typeText = item.GetGenericArguments()?[0]?.Name;
                        }
                    }
                }

            }

            // var editor = UnityEditor.Editor.CreateEditor()
            var labelView = new Label() { text = "TestValue" };

            var wrapper = this.CreateSOWrapper<Wapper>();
            wrapper.Value = instance;

            SerializedObject serializedObject = new SerializedObject(wrapper);
            var prop = serializedObject.FindProperty("Value");
            var propertyField = new PropertyField(prop, "Value");
            propertyField.BindProperty(prop);
            Body.Add(propertyField);

            //var editor = UnityEditor.Editor.CreateEditor(wrapper);
            //var imgui = new IMGUIContainer(() => { editor.OnInspectorGUI(); });
            //主题颜色冲突，不知到怎么控制主题。
            //Body.Add(imgui);
        }

        public class Wapper : ScriptableObject
        {
            [SerializeReference]
            public IRefable Value;
        }

        public override void Select(VisualElement selectionContainer, bool additive)
        {
            base.Select(selectionContainer, additive);
            this.LogMethodName();
        }

        public override void OnSelected()
        {
            base.OnSelected();
            this.LogMethodName();
        }

        public override void OnUnselected()
        {
            base.OnUnselected();
            this.LogMethodName();
        }
    }


}

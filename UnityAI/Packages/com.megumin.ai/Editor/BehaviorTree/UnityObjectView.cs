using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace Megumin.GameFramework.AI.BehaviorTree.Editor
{
    //Copy from Unity C# reference source
    //https://github.com/Unity-Technologies/UnityCsReference/blob/master/Editor/Mono/UIElements/Controls/ObjectField.cs
    public class UnityObjectView : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<UnityObjectView, UxmlTraits> { }

        private readonly Image m_ObjectIcon;
        private readonly Label m_ObjectLabel;

        static readonly string ussClassName = "unity-object-field-display";
        static readonly string ussClassNameInput = "unity-object-field__input";
        static readonly string iconUssClassName = ussClassName + "__icon";
        static readonly string labelUssClassName = ussClassName + "__label";
        private Object value;

        public Object Value
        {
            get => value;
            set
            {
                this.value = value;
                Update();
            }
        }

        public UnityObjectView()
        {
            AddToClassList(ussClassName);
            AddToClassList(ussClassNameInput);
            m_ObjectIcon = new Image { scaleMode = ScaleMode.ScaleAndCrop, pickingMode = PickingMode.Ignore };
            m_ObjectIcon.AddToClassList(iconUssClassName);
            m_ObjectLabel = new Label { pickingMode = PickingMode.Ignore };
            m_ObjectLabel.AddToClassList(labelUssClassName);

            Update();

            Add(m_ObjectIcon);
            Add(m_ObjectLabel);
        }

        public void Update()
        {
            var type = Value?.GetType() ?? typeof(Object);
            GUIContent content = EditorGUIUtility.ObjectContent(Value, type);
            m_ObjectIcon.image = content.image;
            m_ObjectLabel.text = content.text;
        }

        protected override void ExecuteDefaultActionAtTarget(EventBase evt)
        {
            base.ExecuteDefaultActionAtTarget(evt);

            if (evt == null)
            {
                return;
            }

            if ((evt as MouseDownEvent)?.button == (int)MouseButton.LeftMouse)
                OnMouseDown(evt as MouseDownEvent);
        }

        private void OnMouseDown(MouseDownEvent evt)
        {
            Object actualTargetObject = Value;
            Component com = actualTargetObject as Component;
            if (com)
                actualTargetObject = com.gameObject;

            if (actualTargetObject == null)
                return;

            // One click shows where the referenced object is, or pops up a preview
            if (evt.clickCount == 1)
            {
                // ping object
                bool anyModifiersPressed = evt.shiftKey || evt.ctrlKey;
                if (!anyModifiersPressed && actualTargetObject)
                {
                    EditorGUIUtility.PingObject(actualTargetObject);
                }
                evt.StopPropagation();
            }
            // Double click opens the asset in external app or changes selection to referenced object
            else if (evt.clickCount == 2)
            {
                if (actualTargetObject)
                {
                    AssetDatabase.OpenAsset(actualTargetObject);
                    GUIUtility.ExitGUI();
                }
                evt.StopPropagation();
            }
        }
    }
}

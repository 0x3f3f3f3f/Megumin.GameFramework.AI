using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using System;
using UnityEditor;

namespace Megumin.GameFramework.AI.BehaviorTree.Editor
{
    public class BehaviorTreeNodeView : Node
    {

        public new class UxmlFactory : UxmlFactory<BehaviorTreeNodeView, UxmlTraits> { }

        /// <summary>
        /// 没办法，基类只接受路径。
        /// </summary>
        public BehaviorTreeNodeView()
            : base(AssetDatabase.GetAssetPath(Resources.Load<VisualTreeAsset>("BehaviorTreeNodeView")))
        {
            UseDefaultStyling();
            StyleSheet styleSheet = Resources.Load<StyleSheet>("BehaviorTreeNodeView");
            styleSheets.Add(styleSheet);

            var inport = Port.Create<Edge>(Orientation.Vertical, Direction.Input, Port.Capacity.Single, typeof(object));
            var outport = Port.Create<Edge>(Orientation.Vertical, Direction.Output, Port.Capacity.Multi, typeof(object));
            inputContainer.Add(inport);
            outputContainer.Add(outport);


            so = ScriptableObject.CreateInstance<TestSO>();
        }


        private ScriptableObject so;
        public override void Select(VisualElement selectionContainer, bool additive)
        {
            base.Select(selectionContainer, additive);
            Debug.Log(title);
            Selection.activeObject = so;
        }
    }

    public class TestSO : ScriptableObject
    {
        public string TestName = Guid.NewGuid().ToString();
    }

    public class TestSO2<T>: ScriptableObject
    {
        public T Node;
    }
}

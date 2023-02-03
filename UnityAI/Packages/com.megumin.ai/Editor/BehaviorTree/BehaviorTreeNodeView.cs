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
        }


        private NodeWapper so;

        public BehaviorTreeView TreeView { get; internal set; }

        public override void Select(VisualElement selectionContainer, bool additive)
        {
            base.Select(selectionContainer, additive);
            Debug.Log(title);
            if (so)
            {
                Selection.activeObject = so;
            }
        }

        public override void SetPosition(Rect newPos)
        {
            base.SetPosition(newPos);
        }

        internal void SetNode(BTNode node)
        {
            if (node == null)
            {
                title = "TestNode";
                name = "testNode";
            }
            else
            {
                var type = node.GetType();
                title = type.Name;
                so = new NodeWapper();
                so.Node = node;
            }

            var inport = Port.Create<Edge>(Orientation.Vertical, Direction.Input, Port.Capacity.Single, typeof(object));
            var outport = Port.Create<Edge>(Orientation.Vertical, Direction.Output, Port.Capacity.Multi, typeof(object));
            inputContainer.Add(inport);
            outputContainer.Add(outport);
        }
    }

    public class NodeWapper : ScriptableObject
    {
        [SerializeReference]
        public BTNode Node;
    }
}

using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Megumin.GameFramework.AI.BehaviorTree.Editor
{
    public class BehaviorTreeEditor : EditorWindow
    {
        [SerializeField]
        private VisualTreeAsset m_VisualTreeAsset = default;

        [MenuItem("Tools/BehaviorTreeEditor")]
        public static void ShowExample()
        {
            BehaviorTreeEditor wnd = GetWindow<BehaviorTreeEditor>();
            wnd.titleContent = new GUIContent("BehaviorTreeEditor");
        }

        public void CreateGUI()
        {
            VisualElement root = rootVisualElement;

            // Instantiate UXML
            VisualElement labelFromUXML = m_VisualTreeAsset.Instantiate();
            labelFromUXML.name = "BehaviorTreeEditor";
            labelFromUXML.StretchToParentSize();
            root.Add(labelFromUXML);


            //var content = new VisualElement();
            //root.Add(content);
            //var tree = new BehaviourTreeView();
            //content.Add(tree); 
            //var content = root.Q<VisualElement>("Content");
            //content.Add(new BehaviourTreeView());
            //content.StretchToParentSize();
        }
    }
}


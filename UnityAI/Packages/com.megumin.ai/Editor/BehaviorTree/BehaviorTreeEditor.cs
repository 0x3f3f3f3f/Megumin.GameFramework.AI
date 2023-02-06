using System;
using Megumin.GameFramework.AI.Editor;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Megumin.GameFramework.AI.BehaviorTree.Editor
{
    public class BehaviorTreeEditor : EditorWindow
    {
        [OnOpenAsset(10)]
        public static bool OnOpenAsset(int instanceID, int line, int column)
        {
            var asset = EditorUtility.InstanceIDToObject(instanceID);

            if (asset is BehaviorTreeAsset behaviorTreeAsset)
            {
                var wnd = GetWindow(behaviorTreeAsset);
                wnd.SelectTree(behaviorTreeAsset);
                wnd.UpdateTitle();
                return true;
            }

            //TODO Json

            return false;
        }

        [SerializeField]
        private VisualTreeAsset m_VisualTreeAsset = default;

        public BehaviorTreeView TreeView { get; private set; }
        public BehaviorTreeAsset CurrentAsset { get; private set; }

        [MenuItem("Megumin AI/BehaviorTreeEditor")]
        public static void ShowExample()
        {
            var wnd = GetWindow();
            wnd.UpdateTitle();
        }

        private static BehaviorTreeEditor GetWindow(UnityEngine.Object asset = null)
        {
            BehaviorTreeEditor[] array = Resources.FindObjectsOfTypeAll(typeof(BehaviorTreeEditor)) as BehaviorTreeEditor[];
            if (array != null)
            {
                foreach (var item in array)
                {
                    if (item)
                    {
                        if (item.CurrentAsset == asset)
                        {
                            Debug.Log($"找到匹配的已打开EditorWindow {asset}");
                            item.Focus();
                            return item;
                        }
                    }
                }
            }

            BehaviorTreeEditor wnd = CreateWindow<BehaviorTreeEditor>(typeof(BehaviorTreeEditor), typeof(SceneView));

            return wnd;
        }

        public void UpdateTitle()
        {
            if (CurrentAsset)
            {
                this.titleContent = new GUIContent(CurrentAsset.name);
            }
            else
            {
                this.titleContent = new GUIContent("BehaviorTreeEditor");
            }
        }

        public void Update()
        {
            
        }

        public void CreateGUI()
        {
            VisualElement root = rootVisualElement;
            root.AddToClassList("behaviorTreeEditor");

            // Instantiate UXML
            //VisualElement labelFromUXML = m_VisualTreeAsset.Instantiate();
            //labelFromUXML.name = "BehaviorTreeEditor";
            //labelFromUXML.StretchToParentSize();
            //root.Add(labelFromUXML);

            ///CloneTree可以避免生成TemplateContainer
            m_VisualTreeAsset.CloneTree(root);

            TreeView = root.Q<BehaviorTreeView>("behaviorTreeView");
            TreeView.EditorWindow = this;

            CreateTopbar();
        }

        static MySetting<bool> showFloatingTip = new MySetting<bool>("behaviorTreeEditor.showFloatingTip", true, SettingsScope.User);

        private void CreateTopbar()
        {
            VisualElement root = rootVisualElement;
            var toolbar = root.Q<Toolbar>("toolbar");

            var save = root.Q<ToolbarButton>("saveAsset");
            save.clicked += SaveAsset;

            var saveAs = root.Q<ToolbarMenu>("saveAs");
            saveAs.menu.AppendAction("Save as Json", SaveTreeAsJson, a => DropdownMenuAction.Status.Normal);
            saveAs.menu.AppendAction("Save as ScriptObject", SaveTreeAsScriptObject, a => DropdownMenuAction.Status.Normal);

            var file = root.Q<ToolbarMenu>("file");
            file.menu.AppendAction("Default is never shown", a => { }, a => DropdownMenuAction.Status.None);
            file.menu.AppendAction("Normal file", a => { }, a => DropdownMenuAction.Status.Normal);
            file.menu.AppendAction("Hidden is never shown", a => { }, a => DropdownMenuAction.Status.Hidden);
            file.menu.AppendAction("Checked file", a => { }, a => DropdownMenuAction.Status.Checked);
            file.menu.AppendAction("Disabled file", a => { }, a => DropdownMenuAction.Status.Disabled);
            file.menu.AppendAction("Disabled and checked file", a => { }, a => DropdownMenuAction.Status.Disabled | DropdownMenuAction.Status.Checked);

            file.menu.AppendAction("Save", SaveTree, a => DropdownMenuAction.Status.Normal);

            var test1 = root.Q<ToolbarButton>("test1");
            test1.clicked += () =>
            {
                hasUnsavedChanges = true;
                saveChangesMessage = "有未保存改动";
            };

            var test2 = root.Q<ToolbarButton>("test2");
            test2.clicked += () =>
            {
                hasUnsavedChanges = false;
            };

            var test3 = root.Q<ToolbarButton>("test3");
            test3.clicked += () =>
            {

            };

            var showTree = root.Q<ToolbarButton>("showTreeWapper");
            showTree.clicked += () => { TreeView?.InspectorShowWapper(); };

            var showFloatingTipToggle = root.Q<ToolbarToggle>("showFloatingTip");
            showFloatingTipToggle.value = showFloatingTip.value;
            TreeView.FloatingTip.Show(showFloatingTip.value);

            showFloatingTipToggle.RegisterValueChangedCallback(evt =>
            {
                showFloatingTip.SetValue(evt.newValue);
                TreeView.FloatingTip.Show(evt.newValue);
            });
        }

        private void SaveAsset()
        {
            Debug.Log(1);
        }

        private void SaveTreeAsScriptObject(DropdownMenuAction obj)
        {
            var path = EditorUtility.SaveFilePanelInProject("保存", "BTtree", "asset", "test");
            if (!string.IsNullOrEmpty(path))
            {
                Debug.Log(path);
                var tree = ScriptableObject.CreateInstance<BehaviorTreeAsset>();
                AssetDatabase.CreateAsset(tree, path);
                AssetDatabase.Refresh();
            }
        }

        private void SaveTreeAsJson(DropdownMenuAction obj)
        {
            var path = EditorUtility.SaveFilePanelInProject("保存", "BTJson", "json", "test");
            if (!string.IsNullOrEmpty(path))
            {
                Debug.Log(path);
                TextAsset json = new TextAsset("{Tree}");
                AssetDatabase.CreateAsset(json, path);
                AssetDatabase.Refresh();
            }
        }

        private void SaveTree(DropdownMenuAction obj)
        {
            throw new NotImplementedException();
        }

        public void OnEnable()
        {

        }

        private void OnDestroy()
        {
            TreeView?.Dispose();
        }

        public void SelectTree(BehaviorTreeAsset behaviorTreeAsset)
        {
            this.LogFuncName();
            this.CurrentAsset = behaviorTreeAsset;
            TreeView.ReloadView();
        }

        public override void DiscardChanges()
        {
            base.DiscardChanges();
            this.LogFuncName();
        }

        public override void SaveChanges()
        {
            base.SaveChanges();
            this.LogFuncName();
        }
    }
}


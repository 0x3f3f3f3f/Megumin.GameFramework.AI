using System;
using System.Collections.Generic;
using System.Linq;
using Megumin.GameFramework.AI.Editor;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.Experimental.GraphView;
using UnityEditor.SettingsManagement;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Megumin.GameFramework.AI.BehaviorTree.Editor
{
    // Usually you will only have a single Settings instance, so it is convenient to define a UserSetting<T> implementation
    // that points to your instance. In this way you avoid having to pass the Settings parameter in setting field definitions.
    internal class MySetting<T> : UserSetting<T>
    {
        public string FriendKey { get; set; }
        public MySetting(string key, T value, SettingsScope scope = SettingsScope.Project)
            : base(MySettingsManager.instance, $"behaviorTreeEditor_{key}", value, scope)
        {
            FriendKey = key;
        }
    }

    public partial class BehaviorTreeEditor : EditorWindow
    {

        static List<MySetting<bool>> MySettingPrefs = new()
        {
            new MySetting<bool>("FloatingTip", true, SettingsScope.User),
            new MySetting<bool>("DecoratorMarker", true, SettingsScope.User),
            new MySetting<bool>("NodeIcon", true, SettingsScope.User),
            new MySetting<bool>("DecoratorIcon", true, SettingsScope.User),
            new MySetting<bool>("ToolTip", true, SettingsScope.User),
            new MySetting<bool>("MiniMap", false, SettingsScope.User),
            new MySetting<bool>("TODO", false, SettingsScope.User),
        };

        internal readonly static MySetting<Rect> BlackboardLayout
            = new MySetting<Rect>("BlackboardLayout", new Rect(0, 0, 200, 400), SettingsScope.User);

        /// <summary>
        /// 是否显示还还没有实现的Feature。默认是隐藏，否则会给用户造成困惑为什么变灰点不了。
        /// </summary>
        /// <returns></returns>
        public static DropdownMenuAction.Status TODO
        {
            get
            {
                return MySettingPrefs[6].value ? DropdownMenuAction.Status.Disabled : DropdownMenuAction.Status.Hidden;
            }
        }

        [OnOpenAsset(10)]
        public static bool OnOpenAsset(int instanceID, int line, int column)
        {
            var asset = EditorUtility.InstanceIDToObject(instanceID);

            if (asset is BehaviorTreeAsset behaviorTreeAsset)
            {
                var wnd = GetWindow(behaviorTreeAsset);
                wnd.SelectTree(behaviorTreeAsset);
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
        }

        private static BehaviorTreeEditor GetWindow(UnityEngine.Object asset = null)
        {
            BehaviorTreeEditor[] array = Resources.FindObjectsOfTypeAll(typeof(BehaviorTreeEditor)) as BehaviorTreeEditor[];
            if (array != null)
            {
                BehaviorTreeEditor emptyEditor = null;
                foreach (var item in array)
                {
                    if (item)
                    {
                        if (!emptyEditor && !item.CurrentAsset && item.TreeView?.SOTree?.Tree == null)
                        {
                            //找到一个打开的空的Editor
                            emptyEditor = item;
                        }

                        if (item.CurrentAsset == asset)
                        {
                            Debug.Log($"找到匹配的已打开EditorWindow {asset}");
                            item.Focus();
                            item.UpdateTitle();
                            item.UpdateHasUnsavedChanges();
                            return item;
                        }
                    }
                }

                if (emptyEditor)
                {
                    return emptyEditor;
                }
            }

            BehaviorTreeEditor wnd = CreateWindow<BehaviorTreeEditor>(typeof(BehaviorTreeEditor), typeof(SceneView));

            return wnd;
        }

        public void UpdateTitle()
        {
            if (CurrentAsset)
            {
                string title = CurrentAsset.name;
                if (IsDebugMode)
                {
                    title = "[Debug]  " + title;
                }
                this.titleContent = new GUIContent(title);
            }
            else
            {
                this.titleContent = new GUIContent("BehaviorTreeEditor");
            }
        }

        public void UpdateSaveMessage()
        {
            if (CurrentAsset)
            {
                saveChangesMessage = $"{CurrentAsset.name} 有未保存改动";
            }
            else
            {
                saveChangesMessage = $"当前窗口有未保存改动";
            }
        }

        public void UpdateHasUnsavedChanges()
        {
            if (TreeView?.SOTree == null)
            {
                hasUnsavedChanges = false;
            }
            else
            {
                hasUnsavedChanges = TreeView.SOTree.ChangeVersion != SaveVersion;
            }

            UpdateSaveMessage();
            //this.LogMethodName(hasUnsavedChanges);
        }

        public void Update()
        {

        }

        public void CreateGUI()
        {
            this.LogMethodName(CurrentAsset, TreeView?.SOTree);
            VisualElement root = rootVisualElement;
            root.AddToClassList("behaviorTreeEditor");

            root.RegisterCallback<TooltipEvent>(evt =>
            {
                if (!MySettingPrefs[4])
                {
                    //关闭TooltipEvent
                    evt.StopImmediatePropagation();
                }
            }, TrickleDown.TrickleDown);

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

            //应用默认用户首选项
            foreach (var item in MySettingPrefs)
            {
                SetSettingValueClass(item);
            }

            AllActiveEditor.Add(this);

            if (CurrentAsset)
            {
                //通常重载时被触发。
                EditorReloading();
            }

            UpdateTitle();
        }

        private void EditorReloading()
        {
            if (Application.isPlaying)
            {
                Debug.Log("编辑器运行 导致窗口重载");
                DebugSearchInstance();
            }
            else
            {
                Debug.Log("脚本重新编译 导致窗口重载");
                AssetDatabase.OpenAsset(CurrentAsset);
            }
        }

        private void CreateTopbar()
        {
            VisualElement root = rootVisualElement;
            var toolbar = root.Q<Toolbar>("toolbar");

            var save = root.Q<ToolbarButton>("saveAsset");
            save.clicked += SaveAsset;

            var saveAs = root.Q<ToolbarMenu>("saveAs");
            saveAs.menu.AppendAction("Save as Json", SaveTreeAsJson, a => DropdownMenuAction.Status.Normal);
            saveAs.menu.AppendAction("Save as ScriptObject",
                                     a => CreateScriptObjectTreeAssset(),
                                     a => DropdownMenuAction.Status.Normal);

            var showInProject = root.Q<ToolbarButton>("showInProject");
            showInProject.clicked += ShowInProject;

            var file = root.Q<ToolbarMenu>("file");
            file.menu.AppendAction("Default is never shown", a => { }, a => DropdownMenuAction.Status.None);
            file.menu.AppendAction("Normal file", a => { }, a => DropdownMenuAction.Status.Normal);
            file.menu.AppendAction("Hidden is never shown", a => { }, a => DropdownMenuAction.Status.Hidden);
            file.menu.AppendAction("Checked file", a => { }, a => DropdownMenuAction.Status.Checked);
            file.menu.AppendAction("Disabled file", a => { }, a => DropdownMenuAction.Status.Disabled);
            file.menu.AppendAction("Disabled and checked file", a => { }, a => DropdownMenuAction.Status.Disabled | DropdownMenuAction.Status.Checked);

            file.menu.AppendAction("Save", a => SaveAsset(), a => DropdownMenuAction.Status.Normal);

            var prefs = root.Q<ToolbarMenu>("prefs");

            prefs.menu.AppendAction("Reset All Prefs",
                                    a =>
                                    {
                                        foreach (var item in MySettingPrefs)
                                        {
                                            item.Reset();
                                        }
                                        BlackboardLayout.Reset();
                                        Debug.Log("Reset All Prefs");
                                    },
                                    DropdownMenuAction.Status.Normal);

            foreach (var item in MySettingPrefs)
            {
                prefs.menu.AppendAction(item, item.FriendKey, SetSettingValueClass);
            }

            var showTree = root.Q<ToolbarButton>("showTreeWapper");
            showTree.clicked += () => TreeView?.InspectorShowWapper();

            var reloadView = root.Q<ToolbarButton>("reloadView");
            reloadView.clicked += () =>
            {
                TreeView?.ReloadView(true);
                UpdateTitle();
                UpdateHasUnsavedChanges();
            };

            var test1 = root.Q<ToolbarButton>("test1");
            test1.clicked += () =>
            {
                var asset = Resources.FindObjectsOfTypeAll<BehaviorTreeAsset>()
                                     .FirstOrDefault(elem => elem.name == "BTtree");
                AssetDatabase.OpenAsset(asset);
            };

            var test2 = root.Q<ToolbarButton>("test2");
            test2.clicked += () =>
            {
                var asset = Resources.FindObjectsOfTypeAll<BehaviorTreeAsset>()
                                     .FirstOrDefault(elem => elem.name == "BTtree 1");
                AssetDatabase.OpenAsset(asset);
            };

            var test3 = root.Q<ToolbarButton>("test3");
            test3.clicked += () =>
            {

            };

            //var showFloatingTipToggle = root.Q<ToolbarToggle>("showFloatingTip");
            //showFloatingTipToggle.value = showFloatingTip.value;
            //TreeView.FloatingTip.Show(showFloatingTip.value);

            //showFloatingTipToggle.RegisterValueChangedCallback(evt =>
            //{
            //    showFloatingTip.SetValue(evt.newValue);
            //    TreeView.FloatingTip.Show(evt.newValue);
            //});
        }

        internal void SetSettingValueClass(UserSetting<bool> setting)
        {
            if (setting is MySetting<bool> mysetting)
            {
                this.rootVisualElement.SetToClassList($"disable_{mysetting.FriendKey}", !setting);
            }
            else
            {
                this.rootVisualElement.SetToClassList($"disable_{setting.key}", !setting);
            }
        }

        public void ShowInProject()
        {
            if (CurrentAsset)
            {
                Selection.activeObject = CurrentAsset;
            }
        }

        public int SaveVersion = 0;

        double lastSaveClick;
        public void SaveAsset()
        {
            double delta = EditorApplication.timeSinceStartup - lastSaveClick;
            if (delta > 0.5 || delta < 0)
            {
                if (delta > 0)
                {
                    lastSaveClick = EditorApplication.timeSinceStartup;
                }
                SaveAsset(false);
            }
            else
            {
                //短时间内多次点击，强制保存
                lastSaveClick = EditorApplication.timeSinceStartup + 3;
                SaveAsset(true);
            }
        }

        public void SaveAsset(bool force = false)
        {
            if (TreeView?.SOTree?.ChangeVersion == SaveVersion && !force)
            {
                Debug.Log($"没有需要保存的改动。");
                return;
            }

            if (!CurrentAsset)
            {
                CurrentAsset = CreateScriptObjectTreeAssset();
            }

            if (!CurrentAsset)
            {
                Debug.Log($"保存资源失败，没有找到Asset文件");
                return;
            }

            var success = CurrentAsset.SaveTree(TreeView.Tree);
            if (success)
            {
                EditorUtility.SetDirty(CurrentAsset);
                AssetDatabase.SaveAssetIfDirty(CurrentAsset);
                AssetDatabase.Refresh();

                Debug.Log($"保存资源成功");
                SaveVersion = TreeView.SOTree.ChangeVersion;
                UpdateHasUnsavedChanges();
            }
            else
            {
                Debug.Log($"保存资源失败");
            }
        }

        public BehaviorTreeAsset CreateScriptObjectTreeAssset()
        {
            var path = EditorUtility.SaveFilePanelInProject("保存", "BTtree", "asset", "test");
            if (!string.IsNullOrEmpty(path))
            {
                Debug.Log(path);
                var tree = ScriptableObject.CreateInstance<BehaviorTreeAsset>();
                AssetDatabase.CreateAsset(tree, path);
                AssetDatabase.Refresh();
                return tree;
            }

            return null;
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

        public void OnEnable()
        {
            this.LogMethodName(TreeView);

            if (BehaviorTreeManager.TreeDebugger == null)
            {
                BehaviorTreeManager.TreeDebugger = new BehaviorTreeEditorDebugger();
            }
        }

        private void OnDestroy()
        {
            this.LogMethodName(TreeView);
            AllActiveEditor.Remove(this);
            TreeView?.Dispose();
        }

        private void Reset()
        {
            this.LogMethodName(TreeView);
        }

        private void OnDisable()
        {
            this.LogMethodName(TreeView);
            TreeView?.Dispose();
        }

        private void OnProjectChange()
        {
            this.LogMethodName(TreeView);
        }

        public void SelectTree(BehaviorTreeAsset behaviorTreeAsset)
        {
            this.LogMethodName();
            this.CurrentAsset = behaviorTreeAsset;

            if (EditorApplication.isPlaying)
            {
                //debug 模式关联
                //if (BehaviorTreeManager.Instance)
                //{

                //}

                if (Selection.activeGameObject)
                {
                    var runner = Selection.activeGameObject.GetComponent<BehaviorTreeRunner>();
                    BeginDebug(runner);
                    return;
                }
            }

            TreeView?.ReloadView(true);
        }

        public override void DiscardChanges()
        {
            base.DiscardChanges();
            this.LogMethodName();
        }

        public override void SaveChanges()
        {
            base.SaveChanges();
            this.LogMethodName();
        }

        protected override void OnBackingScaleFactorChanged()
        {
            base.OnBackingScaleFactorChanged();
            this.LogMethodName();
        }
    }
}


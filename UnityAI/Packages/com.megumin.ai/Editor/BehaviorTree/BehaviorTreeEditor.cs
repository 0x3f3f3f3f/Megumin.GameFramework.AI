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
    public class MySetting<T> : UserSetting<T>
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
        static BehaviorTreeEditor()
        {
            Megumin.Reflection.TypeCache.CacheAllTypesAsync();
        }

        static List<MySetting<bool>> MySettingPrefs = new()
        {
            new MySetting<bool>("TODO", false, SettingsScope.User),
            new MySetting<bool>("Blackboard", true, SettingsScope.User),
            new MySetting<bool>("GraphInspector", false, SettingsScope.User),
            new MySetting<bool>("MiniMap", false, SettingsScope.User),
            new MySetting<bool>("ToolTip", true, SettingsScope.User),
            new MySetting<bool>("FloatingTip", true, SettingsScope.User),
            new MySetting<bool>("FriendlyZoom", true, SettingsScope.User),
            new MySetting<bool>("NodeIndex", true, SettingsScope.User),
            new MySetting<bool>("NodeIcon", true, SettingsScope.User),
            new MySetting<bool>("NodeDetail", true, SettingsScope.User),
            new MySetting<bool>("DecoratorIcon", true, SettingsScope.User),
            new MySetting<bool>("DecoratorMarker", true, SettingsScope.User),
            new MySetting<bool>("DecoratorDetail", true, SettingsScope.User),
            //new MySetting<bool>("RingGraph", false, SettingsScope.User),
            //new MySetting<bool>("DiamondGraph", false, SettingsScope.User),
        };

        internal readonly static MySetting<Rect> BlackboardLayout
            = new MySetting<Rect>("BlackboardLayout", new Rect(0, 0, 340, 400), SettingsScope.User);

        public static MySetting<bool> ToolTipSetting => MySettingPrefs[4];
        public static MySetting<bool> FriendlyZoomSetting => MySettingPrefs[6];

        /// <summary>
        /// 是否显示还还没有实现的Feature。默认是隐藏，否则会给用户造成困惑为什么变灰点不了。
        /// </summary>
        /// <returns></returns>
        public static DropdownMenuAction.Status TODO
        {
            get
            {
                return MySettingPrefs[0].value ? DropdownMenuAction.Status.Disabled : DropdownMenuAction.Status.Hidden;
            }
        }

        [OnOpenAsset(10)]
        public static bool OnOpenAsset(int instanceID, int line, int column)
        {
            var asset = EditorUtility.InstanceIDToObject(instanceID);

            if (asset is IBehaviorTreeAsset behaviorTreeAsset)
            {
                var wnd = GetWindow(behaviorTreeAsset.AssetObject);
                wnd.SelectTree(behaviorTreeAsset);
                return true;
            }

            //TODO Json

            return false;
        }

        public static bool OnOpenAsset(IBehaviorTreeAsset behaviorTreeAsset, bool forceNewEditor = false)
        {
            if (behaviorTreeAsset is UnityEngine.Object obj && !obj)
            {
                return false;
            }

            if (behaviorTreeAsset != null)
            {
                var wnd = GetWindow(behaviorTreeAsset.AssetObject, forceNewEditor);
                wnd.SelectTree(behaviorTreeAsset);
                return true;
            }

            return false;
        }

        [SerializeField]
        private VisualTreeAsset m_VisualTreeAsset = default;

        public BehaviorTreeView TreeView { get; private set; }
        /// <summary>
        /// 保存资产引用，切换运行模式和重写编译时，保持打开的窗口不变。
        /// </summary>
        /// <remarks>
        /// 原理是EditorWindow本身是一个ScriptableObject，重载时所有静态变量都会重置。
        /// 只有ScriptableObject的序列化对象才能保存下来。
        /// EditorWindow是一个内存SO对象。
        /// </remarks>
        public UnityEngine.Object CurrentAsset_AssetObject;
        public IBehaviorTreeAsset CurrentAsset { get; private set; }
        public UnityObjectView DebugInstanceGameObject { get; private set; }

        [MenuItem("Tools/Megumin/BehaviorTreeEditor")]
        public static void ShowExample()
        {
            var wnd = GetWindow();
        }

        public static BehaviorTreeEditor GetWindow(UnityEngine.Object asset = null, bool forceNewEditor = false)
        {
            BehaviorTreeEditor[] array = Resources.FindObjectsOfTypeAll(typeof(BehaviorTreeEditor)) as BehaviorTreeEditor[];
            if (array != null && forceNewEditor == false)
            {
                BehaviorTreeEditor emptyEditor = null;
                foreach (var item in array)
                {
                    if (item)
                    {
                        if (!emptyEditor && item.CurrentAsset == null && item.TreeView?.SOTree?.Tree == null)
                        {
                            //找到一个打开的空的Editor
                            emptyEditor = item;
                        }

                        if (item.CurrentAsset?.AssetObject == asset)
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

        public static BehaviorTreeEditor GetWindow(BehaviorTree tree,
                                                   bool forceNewEditor = false)
        {
            BehaviorTreeEditor[] array = Resources.FindObjectsOfTypeAll(typeof(BehaviorTreeEditor)) as BehaviorTreeEditor[];
            if (array != null && forceNewEditor == false)
            {
                BehaviorTreeEditor emptyEditor = null;
                foreach (var item in array)
                {
                    if (item)
                    {
                        if (!emptyEditor && item.CurrentAsset == null && item.TreeView?.SOTree?.Tree == null)
                        {
                            //找到一个打开的空的Editor
                            emptyEditor = item;
                        }

                        if (tree != null && item.DebugInstance == tree)
                        {
                            return item;
                        }

                        if (item.IsDebugMode == false && item.IsSameAsset(tree.Asset))
                        {
                            Debug.Log($"找到匹配的已打开EditorWindow");
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
            if (CurrentAsset != null)
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
            if (CurrentAsset != null)
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
            if (BehaviorTreeEditor.EditorLog)
            {
                this.LogMethodName(CurrentAsset, TreeView?.SOTree);
            }

            VisualElement root = rootVisualElement;
            root.AddToClassList("behaviorTreeEditor");

            root.RegisterCallback<TooltipEvent>(evt =>
            {
                if (!ToolTipSetting)
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
            CreateBottomBar();

            //应用默认用户首选项
            foreach (var item in MySettingPrefs)
            {
                SetSettingValueClass(item);
            }

            AllActiveEditor.Add(this);

            EditorReloading();
            UpdateTitle();
        }

        /// <summary>
        /// 通常重载时被触发。
        /// </summary>
        private void EditorReloading()
        {
            if (BehaviorTreeEditor.EditorLog)
            {
                if (Application.isPlaying)
                {
                    Debug.Log("Editor isPlaying.  ReloadWindow.");
                    //Debug.Log("编辑器运行 导致窗口重载");
                }
                else
                {
                    Debug.Log("Script recompilation. ReloadWindow.");
                    //Debug.Log("脚本重新编译 导致窗口重载");
                }
            }

            if (CurrentAsset == null)
            {
                SelectTree(CurrentAsset_AssetObject as IBehaviorTreeAsset);
            }
            else
            {
                SelectTree(CurrentAsset);
            }

            if (Application.isPlaying)
            {
                DebugSearchInstance();
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
            saveAs.menu.AppendAction("Save as BehaviorTreeAsset_1_1_0",
                                     a => CreateScriptObjectTreeAssset<BehaviorTreeAsset_1_1>(),
                                     a => DropdownMenuAction.Status.Normal);

            var showInProject = root.Q<ToolbarButton>("showInProject");
            showInProject.clicked += ShowInProject;

            var file = root.Q<ToolbarMenu>("file");
            file.menu.AppendAction("ShowTreeWapper", a => TreeView?.InspectorShowWapper(), a => DropdownMenuAction.Status.Normal);
            file.menu.AppendAction("GenerateCode", a => GenerateCode(), a => DropdownMenuAction.Status.Normal);
            file.menu.AppendAction("Change GUID", a =>
                                                {
                                                    if (CurrentAsset != null)
                                                    {
                                                        CurrentAsset.GUID = Guid.NewGuid().ToString();
                                                    }
                                                }, a => DropdownMenuAction.Status.Normal);
            file.menu.AppendAction("Save", a => SaveAsset(), a => DropdownMenuAction.Status.Normal);

            var edit = root.Q<ToolbarMenu>("edit");
            edit.menu.AppendAction("Test1", a => { }, a => DropdownMenuAction.Status.Normal);
            edit.menu.AppendAction("Test2", a => { }, a => DropdownMenuAction.Status.Normal);

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

            DebugInstanceGameObject = root.Q<UnityObjectView>("debugInstanceGameObject");

            var blackboardToggle = root.Q<ToolbarToggle>("blackboard");
            blackboardToggle.value = MySettingPrefs[1].value;
            blackboardToggle.RegisterValueChangedCallback(evt =>
            {
                MySettingPrefs[1].SetValue(evt.newValue);
                SetSettingValueClass(MySettingPrefs[1]);
            });

            var graphInspectorToggle = root.Q<ToolbarToggle>("graphInspector");
            graphInspectorToggle.value = MySettingPrefs[2].value;
            graphInspectorToggle.RegisterValueChangedCallback(evt =>
            {
                MySettingPrefs[2].SetValue(evt.newValue);
                SetSettingValueClass(MySettingPrefs[2]);
            });

            var help = root.Q<ToolbarMenu>("help");
            help.menu.AppendAction("Samples",
                a =>
                {
                    System.Diagnostics.Process.Start("https://github.com/KumoKyaku/Megumin.GameFramework.AI.Samples");
                }, a => DropdownMenuAction.Status.Normal);
            help.menu.AppendAction("Wiki",
                a =>
                {
                    System.Diagnostics.Process.Start("https://github.com/KumoKyaku/Megumin.GameFramework.AI.Samples/wiki");
                }, a => DropdownMenuAction.Status.Normal);
            help.menu.AppendAction("Issues",
                a =>
                {
                    System.Diagnostics.Process.Start("https://github.com/KumoKyaku/Megumin.GameFramework.AI.Samples/issues");
                }, a => DropdownMenuAction.Status.Normal);
            help.menu.AppendAction("Forum",
                a =>
                {
                    //System.Diagnostics.Process.Start("https://github.com/KumoKyaku/Megumin.GameFramework.AI.Samples/issues");
                }, a => DropdownMenuAction.Status.Disabled);
        }



        private void CreateBottomBar()
        {
            VisualElement root = rootVisualElement;
            var bottomBar = root.Q<Toolbar>("bottomBar");
            //DebugInstanceGameObject = bottomBar.Q<ObjectField>("debugInstanceGameObject");
            //DebugInstanceGameObject.SetEnabled(false);

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
            if (CurrentAsset != null)
            {
                if (CurrentAsset.AssetObject)
                {
                    Selection.activeObject = CurrentAsset.AssetObject;
                }
            }
        }

        public void OnEnable()
        {
            if (BehaviorTreeEditor.EditorLog)
            {
                this.LogMethodName(TreeView);
            }

            if (BehaviorTreeManager.TreeDebugger == null)
            {
                BehaviorTreeManager.TreeDebugger = new BehaviorTreeEditorDebugger();
            }
        }

        private void OnDestroy()
        {
            if (BehaviorTreeEditor.EditorLog)
            {
                this.LogMethodName(TreeView);
            }

            AllActiveEditor.Remove(this);
            TreeView?.Dispose();
        }

        private void Reset()
        {
            if (BehaviorTreeEditor.EditorLog)
            {
                this.LogMethodName(TreeView);
            }
        }

        private void OnDisable()
        {
            if (BehaviorTreeEditor.EditorLog)
            {
                this.LogMethodName(TreeView);
            }

            TreeView?.Dispose();
        }

        private void OnProjectChange()
        {
            if (BehaviorTreeEditor.EditorLog)
            {
                this.LogMethodName(TreeView);
            }
        }

        static bool HotTypeAlias = false;
        public void SelectTree(IBehaviorTreeAsset behaviorTreeAsset)
        {
            if (BehaviorTreeEditor.EditorLog)
            {
                this.LogMethodName(TreeView);
            }

            if (HotTypeAlias == false && behaviorTreeAsset != null)
            {
                //在没有行为树文件时不需要触发 缓存别名。防止第一次打开空编辑器卡顿。

                //Megumin.Reflection.TypeCache.CacheAssembly(typeof(int).Assembly);
                //Megumin.Reflection.TypeCache.CacheAssembly(typeof(GameObject).Assembly);
                //因为要处理别名，无论如何都会触发CacheAllType.
                //第一次反序列化之前处理一下类型别名。防止节点改名后报错。
                Megumin.Reflection.TypeCache.HotTypeAliasDerivedFrom<ITreeElement>();
                HotTypeAlias = true;
            }

            bool isChangeTree = SetTreeAsset(behaviorTreeAsset);
            if (EditorApplication.isPlaying)
            {
                //debug 模式关联
                if (Selection.activeGameObject)
                {
                    var runner = Selection.activeGameObject.GetComponent<BehaviorTreeRunner>();
                    if (runner)
                    {
                        if (CanAttachDebug(runner.BehaviourTree))
                        {
                            BeginDebug(runner.BehaviourTree);
                            return;
                        }
                    }
                }
            }

            if (TreeView != null)
            {
                TreeView.ReloadView(true);
                if (isChangeTree)
                {
                    //新打开的行为树，剧中所有节点
                    //TreeView.DelayFrameAll();
                }
            }
        }

        public bool SetTreeAsset(IBehaviorTreeAsset behaviorTreeAsset)
        {
            bool isChangeTree = IsSameAsset(behaviorTreeAsset);

            this.CurrentAsset = behaviorTreeAsset;
            this.CurrentAsset_AssetObject = behaviorTreeAsset?.AssetObject;
            return isChangeTree;
        }

        public bool IsSameAsset(IBehaviorTreeAsset behaviorTreeAsset)
        {
            return CurrentAsset != behaviorTreeAsset
                            || CurrentAsset_AssetObject != behaviorTreeAsset?.AssetObject;
        }

        public override void DiscardChanges()
        {
            base.DiscardChanges();
            if (BehaviorTreeEditor.EditorLog)
            {
                this.LogMethodName(TreeView);
            }
        }

        public override void SaveChanges()
        {
            base.SaveChanges();
            if (BehaviorTreeEditor.EditorLog)
            {
                this.LogMethodName(TreeView);
            }
        }

        protected override void OnBackingScaleFactorChanged()
        {
            base.OnBackingScaleFactorChanged();
            if (BehaviorTreeEditor.EditorLog)
            {
                this.LogMethodName(TreeView);
            }
        }
    }
}


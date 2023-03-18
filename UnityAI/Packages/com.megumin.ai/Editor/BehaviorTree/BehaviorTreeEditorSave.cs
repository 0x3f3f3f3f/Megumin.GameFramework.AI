using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Megumin.GameFramework.AI.BehaviorTree.Editor
{
    public partial class BehaviorTreeEditor
    {

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
                var treeAsset = ScriptableObject.CreateInstance<BehaviorTreeAsset>();
                treeAsset.SaveTree(TreeView.Tree);
                AssetDatabase.CreateAsset(treeAsset, path);
                AssetDatabase.Refresh();

                SelectTree(treeAsset);
                return treeAsset;
            }

            return null;
        }

        public BehaviorTreeAsset_1_0_1 CreateBehaviorTreeAsset_1_0_1()
        {
            var path = EditorUtility.SaveFilePanelInProject("保存", "BTtree", "asset", "test");
            if (!string.IsNullOrEmpty(path))
            {
                Debug.Log(path);
                var treeAsset = ScriptableObject.CreateInstance<BehaviorTreeAsset_1_0_1>();
                treeAsset.Se(TreeView.Tree);
                AssetDatabase.CreateAsset(treeAsset, path);
                AssetDatabase.Refresh();

                //SelectTree(treeAsset);
                return treeAsset;
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

    }
}




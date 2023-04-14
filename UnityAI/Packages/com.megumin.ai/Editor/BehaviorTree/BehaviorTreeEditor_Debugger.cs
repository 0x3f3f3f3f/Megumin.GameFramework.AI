using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Megumin.GameFramework.AI.Editor;
using UnityEditor;
using UnityEngine;

namespace Megumin.GameFramework.AI.BehaviorTree.Editor
{
    internal class BehaviorTreeEditorDebugger : ITreeDebugger
    {
        public void PostTick()
        {
            //在所有打开的编辑器中找到 空闲的，符合当前tree的编辑器
            foreach (var item in BehaviorTreeEditor.AllActiveEditor)
            {
                if (item.IsDebugMode && item.hasFocus)
                {
                    item.OnPostTick();
                }
            }
        }

        public void AddDebugInstanceTree(BehaviorTree tree)
        {
            if (tree == null)
            {
                return;
            }

            if (EditorApplication.isPlaying)
            {
                if (BehaviorTreeEditor.AllActiveEditor.Any(elem => elem.DebugInstance == tree))
                {
                    return;
                }

                //在所有打开的编辑器中找到 空闲的，符合当前tree的编辑器
                foreach (var item in BehaviorTreeEditor.AllActiveEditor)
                {
                    if (item.CurrentAsset.AssetObject == tree.Asset.AssetObject)
                    {
                        if (item.IsDebugMode)
                        {

                        }
                        else
                        {
                            item.BeginDebug(tree);
                        }
                    }
                }
            }
            else
            {
                BehaviorTreeEditor.OnOpenAsset(tree.Asset);
            }
        }

        public void StopDebug()
        {
            foreach (var item in BehaviorTreeEditor.AllActiveEditor)
            {
                if (item.IsDebugMode)
                {
                    item.EndDebug();
                }
            }
        }
    }

    public partial class BehaviorTreeEditor
    {
        public static HashSet<BehaviorTreeEditor> AllActiveEditor { get; } = new();
        public bool IsDebugMode { get; set; }
        public bool IsRemoteDebug { get; set; }
        public bool IsIdel => CurrentAsset == null;

        public BehaviorTree DebugInstance { get; set; }
        internal void BeginDebug(BehaviorTree tree)
        {
            this.LogMethodName();
            IsDebugMode = true;
            DebugInstance = tree;
            rootVisualElement.SetToClassList(UssClassConst.debugMode, IsDebugMode);
            var so = TreeView.CreateSOWrapperIfNull();
            so.Tree = tree;

            if (DebugInstanceGameObject != null)
            {
                if (tree.Agent is UnityEngine.Object agentObj)
                {
                    DebugInstanceGameObject.Value = agentObj;
                }
            }

            TreeView.ReloadView(true);
            OnPostTick();
            UpdateTitle();
        }

        internal void EndDebug()
        {
            IsDebugMode = false;
            DebugInstance = null;
            rootVisualElement.SetToClassList(UssClassConst.debugMode, IsDebugMode);
            TreeView.SOTree.Tree = null;

            if (DebugInstanceGameObject != null)
            {
                DebugInstanceGameObject.Value = null;
            }

            TreeView.ReloadView(true);
            UpdateTitle();
        }

        private void DebugSearchInstance()
        {
            if (IsDebugMode)
            {
                return;
            }

            if (BehaviorTreeManager.Instance)
            {
                var list = BehaviorTreeManager.Instance.AllTree;
                foreach (var item in list)
                {
                    if (CanAttachDebug(item))
                    {
                        BeginDebug(item);
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// 能不能进入Debug模式
        /// </summary>
        /// <param name="behaviorTreeRunner"></param>
        /// <returns></returns>
        public bool CanAttachDebug(BehaviorTree tree)
        {
            if (tree != null && tree.Asset.AssetObject == CurrentAsset?.AssetObject)
            {
                return true;
            }

            return false;
        }


        [MenuItem("Tools/Megumin/Log All Active BehaviorTreeEditor")]
        public static void TestButton()
        {
            foreach (var item in BehaviorTreeEditor.AllActiveEditor)
            {
                Debug.Log(item);
            }
        }

        /// <summary>
        /// BehaviorTreeManager Tick后被调用
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        internal void OnPostTick()
        {
            TreeView?.OnPostTick();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
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
            var so = TreeView.CreateSOWrapperIfNull();
            so.Tree = tree;
            TreeView.ReloadView(true);
            UpdateTitle();
        }

        internal void EndDebug()
        {
            IsDebugMode = false;
            DebugInstance = null;
            TreeView.SOTree.Tree = null;
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


        [MenuItem("Megumin/Log All Active BehaviorTreeEditor")]
        public static void TestButton()
        {
            foreach (var item in BehaviorTreeEditor.AllActiveEditor)
            {
                Debug.Log(item.ToStringReflection());
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

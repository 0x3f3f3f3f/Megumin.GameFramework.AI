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

        public void AddTreeRunner(BehaviorTreeRunner behaviorTreeRunner)
        {
            behaviorTreeRunner.LogMethodName();
            //在所有打开的编辑器中找到 空闲的，符合当前tree的编辑器
            foreach (var item in BehaviorTreeEditor.AllActiveEditor)
            {
                if (item.CurrentAsset.AssetObject == behaviorTreeRunner.BehaviorTreeAsset)
                {
                    if (item.IsDebugMode)
                    {

                    }
                    else
                    {
                        item.BeginDebug(behaviorTreeRunner);
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

        public BehaviorTreeRunner DebugInstance { get; set; }
        internal void BeginDebug(BehaviorTreeRunner behaviorTreeRunner)
        {
            IsDebugMode = true;
            DebugInstance = behaviorTreeRunner;
            var so = TreeView.CreateSOWrapperIfNull();
            so.Tree = behaviorTreeRunner.BehaviourTree;
            TreeView.ReloadView(true);
            UpdateTitle();
        }

        internal void EndDebug()
        {
            IsDebugMode = false;
            TreeView.SOTree.Tree = null;

            TreeView.ReloadView(true);
            UpdateTitle();
        }

        private void DebugSearchInstance()
        {
            if (BehaviorTreeManager.Instance)
            {
                var list = BehaviorTreeManager.Instance.AllTree;
                foreach (var item in list)
                {
                    if (item.BehaviorTreeAsset && item.BehaviourTree != null &&
                        item.BehaviorTreeAsset == CurrentAsset?.AssetObject)
                    {
                        BeginDebug(item);
                        break;
                    }
                }
            }
        }



        [MenuItem("Megumin AI/TestButton")]
        public static void TestButton()
        {
            foreach (var item in BehaviorTreeEditor.AllActiveEditor)
            {
                Debug.Log(item.ToStringReflection());
            }

            Megumin.Serialization.TypeCache.Test();
            //var type = typeof(Dictionary<int, string>);
            //Debug.Log(type.FullName);
            //var type2 = Type.GetType("Dictionary<,>");
            //Debug.Log(type2?.FullName);
            //var name = typeof(Dictionary<,>).FullName;
            //var test = Type.GetType(name);

            //Megumin.Serialization.TypeCache.TryGetGenericAndSpecializedType(type.FullName, out var _, out var _);

            //var name2 = typeof(ParamVariable<>).FullName;
            //var p = Type.GetType(name2);
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

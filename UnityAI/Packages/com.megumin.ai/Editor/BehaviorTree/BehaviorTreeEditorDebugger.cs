using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Megumin.GameFramework.AI.BehaviorTree.Editor
{
    internal class BehaviorTreeEditorDebugger : ITreeDebugger
    {
        public void PostTick()
        {

        }

        public void AddTreeRunner(BehaviorTreeRunner behaviorTreeRunner)
        {
            behaviorTreeRunner.LogMethodName();
            //在所有打开的编辑器中找到 空闲的，符合当前tree的编辑器
            foreach (var item in BehaviorTreeEditor.AllActiveEditor)
            {
                if (item.CurrentAsset == behaviorTreeRunner.BehaviorTreeAsset)
                {
                    if (item.IsDebugMode)
                    {
                        
                    }
                    else
                    {
                        item.IsDebug(behaviorTreeRunner);
                    }
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
        internal void IsDebug(BehaviorTreeRunner behaviorTreeRunner)
        {
            IsDebugMode = true;
            DebugInstance = behaviorTreeRunner;
            var so = TreeView.CreateSOWrapperIfNull();
            so.Tree = behaviorTreeRunner.BehaviourTree;
            UpdateTitle();
            TreeView.ReloadView(true);
        }
    }
}

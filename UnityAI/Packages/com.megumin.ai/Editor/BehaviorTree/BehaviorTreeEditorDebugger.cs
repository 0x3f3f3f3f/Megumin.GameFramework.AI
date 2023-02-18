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
    }

    public partial class BehaviorTreeEditor
    {
        static List<BehaviorTreeEditor> AllActiveEditor = new();
        public bool IsDebugMode { get; set; }
        public bool IsRemoteDebug { get; set; } 
    }
}

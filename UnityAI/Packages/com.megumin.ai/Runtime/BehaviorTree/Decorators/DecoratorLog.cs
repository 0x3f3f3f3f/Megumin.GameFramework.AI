using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Megumin.GameFramework.AI.BehaviorTree
{
    [Icon("console.infoicon@2x")]
    [Category("Debug")]
    internal class DecoratorLog : BTDecorator, IPreDecorator, IPostDecorator, IAbortDecorator
    {
        public bool PreLog = false;
        public bool PostLog = false;
        public bool AbortLog = false;

        public RefVar_String LogStr = new() { value = "Hello world!" };

        public void BeforeNodeEnter(BTNode container)
        {
            if (PreLog)
            {
                Debug.Log($"PreDeco: {container}  {(string)LogStr}");
            }
        }

        public Status AfterNodeExit(Status result, BTNode container)
        {
            if (PostLog)
            {
                Debug.Log($"PostDeco: {container}  {result}  {(string)LogStr}");
            }
            return result;
        }

        public void OnNodeAbort(BTNode container)
        {
            if (AbortLog)
            {
                Debug.Log($"AbortDeco: {container}  {(string)LogStr}");
            }
        }
    }
}

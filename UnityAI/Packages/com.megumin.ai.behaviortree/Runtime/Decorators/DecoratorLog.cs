using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Megumin.Binding;
using UnityEngine;

namespace Megumin.AI.BehaviorTree
{
    /// <summary>
    /// 日志节点，不使用GetLogger机制
    /// </summary>
    [Icon("console.infoicon@2x")]
    [Category("Debug")]
    public class DecoratorLog : BTDecorator, IConditionDecorator, IPreDecorator, IPostDecorator, IAbortDecorator
    {
        [Space]
        public DecoratorPosition DecoratorPosition = DecoratorPosition.None;

        public RefVar_String LogStr = new() { value = "Hello world!" };

        public bool LastCheckResult => true;
        public bool CheckCondition(object options = null)
        {
            if ((DecoratorPosition & DecoratorPosition.Condition) != 0)
            {
                Debug.Log($"Condition: {Owner}  {(string)LogStr}");
            }
            return true;
        }

        public void BeforeNodeEnter(object options = null)
        {
            if ((DecoratorPosition & DecoratorPosition.PreEnter) != 0)
            {
                Debug.Log($"PreDeco: {Owner}  {(string)LogStr}");
            }
        }

        public Status AfterNodeExit(Status result, object options = null)
        {
            if ((DecoratorPosition & DecoratorPosition.PostExit) != 0)
            {
                Debug.Log($"PostDeco: {Owner}  {result}  {(string)LogStr}");
            }
            return result;
        }

        public void OnNodeAbort(object options = null)
        {
            if ((DecoratorPosition & DecoratorPosition.Abort) != 0)
            {
                Debug.Log($"AbortDeco: {Owner}  {(string)LogStr}");
            }
        }
    }
}

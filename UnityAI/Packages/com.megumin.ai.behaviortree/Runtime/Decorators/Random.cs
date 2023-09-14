using System.Collections;
using System.Collections.Generic;
using Megumin.Binding;
using UnityEngine;

namespace Megumin.AI.BehaviorTree
{
    public class Random_Float : BTDecorator, IConditionDecorator, IPreDecorator, IPostDecorator, IAbortDecorator
    {
        [Space]
        public DecoratorPosition DecoratorPosition = DecoratorPosition.None;

        public RefVar_Float SaveTo;

        public bool LastCheckResult => true;
        public bool CheckCondition(object options = null)
        {
            if ((DecoratorPosition & DecoratorPosition.Condition) != 0)
            {
                SaveTo?.SetValue(Random.value);
            }
            return true;
        }


        public void BeforeNodeEnter(object options = null)
        {
            if ((DecoratorPosition & DecoratorPosition.PreEnter) != 0)
            {
                SaveTo?.SetValue(Random.value);
            }
        }

        public Status AfterNodeExit(Status result, object options = null)
        {
            if ((DecoratorPosition & DecoratorPosition.PostExit) != 0)
            {
                SaveTo?.SetValue(Random.value);
            }
            return result;
        }

        public void OnNodeAbort(object options = null)
        {
            if ((DecoratorPosition & DecoratorPosition.Abort) != 0)
            {
                SaveTo?.SetValue(Random.value);
            }
        }
    }
}

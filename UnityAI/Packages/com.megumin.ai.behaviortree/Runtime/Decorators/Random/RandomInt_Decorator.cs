using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Megumin.Binding;
using UnityEngine;

namespace Megumin.AI.BehaviorTree
{
    [DisplayName("RandomInt")]
    public class RandomInt_Decorator : BTDecorator, IConditionDecorator, IPreDecorator, IPostDecorator, IAbortDecorator
    {
        [Space]
        public DecoratorPosition DecoratorPosition = DecoratorPosition.None;

        [Space]
        public bool UseRange = false;
        public RefVar_Int Min;
        public RefVar_Int Max;

        [Space]
        public RefVar_Int SaveTo;

        public bool LastCheckResult => true;
        public bool CheckCondition(object options = null)
        {
            if ((DecoratorPosition & DecoratorPosition.Condition) != 0)
            {
                if (UseRange)
                {
                    SaveTo?.SetValue(Random.Range(Min, Max));
                }
                else
                {
                    SaveTo?.SetValue(((int)Random.value));
                }
            }
            return true;
        }


        public void BeforeNodeEnter(object options = null)
        {
            if ((DecoratorPosition & DecoratorPosition.PreEnter) != 0)
            {
                if (UseRange)
                {
                    SaveTo?.SetValue(Random.Range(Min, Max));
                }
                else
                {
                    SaveTo?.SetValue(((int)Random.value));
                }
            }
        }

        public Status AfterNodeExit(Status result, object options = null)
        {
            if ((DecoratorPosition & DecoratorPosition.PostExit) != 0)
            {
                if (UseRange)
                {
                    SaveTo?.SetValue(Random.Range(Min, Max));
                }
                else
                {
                    SaveTo?.SetValue(((int)Random.value));
                }
            }
            return result;
        }

        public void OnNodeAbort(object options = null)
        {
            if ((DecoratorPosition & DecoratorPosition.Abort) != 0)
            {
                if (UseRange)
                {
                    SaveTo?.SetValue(Random.Range(Min, Max));
                }
                else
                {
                    SaveTo?.SetValue(((int)Random.value));
                }
            }
        }
    }
}

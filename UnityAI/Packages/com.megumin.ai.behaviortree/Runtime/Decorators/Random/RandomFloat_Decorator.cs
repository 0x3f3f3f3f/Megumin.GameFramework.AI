using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Megumin.Binding;
using Megumin.Reflection;
using UnityEngine;

namespace Megumin.AI.BehaviorTree
{
    [DisplayName("RandomFloat")]
    [SerializationAlias("Megumin.AI.BehaviorTree.Random_Float")]
    public class RandomFloat_Decorator : BTDecorator, IConditionDecorator, IPreDecorator, IPostDecorator, IAbortDecorator
    {
        [Space]
        public DecoratorPosition DecoratorPosition = DecoratorPosition.None;

        [Space]
        public bool UseRange = false;
        public RefVar_Float Min;
        public RefVar_Float Max;

        [Space]
        public RefVar_Float SaveTo;

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
                    SaveTo?.SetValue(Random.value);
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
                    SaveTo?.SetValue(Random.value);
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
                    SaveTo?.SetValue(Random.value);
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
                    SaveTo?.SetValue(Random.value);
                }
            }
        }
    }
}

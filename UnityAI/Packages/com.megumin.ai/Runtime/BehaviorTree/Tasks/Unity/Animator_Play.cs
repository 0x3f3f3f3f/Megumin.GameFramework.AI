using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

namespace Megumin.GameFramework.AI.BehaviorTree
{
    [Category("Unity/Animator")]
    public class Animator_Play : BTActionNode<Animator>
    {
        protected override Status OnTick(BTNode from, object options = null)
        {
            MyAgent.Play("TEST");
            return Status.Succeeded;
        }
    }
}

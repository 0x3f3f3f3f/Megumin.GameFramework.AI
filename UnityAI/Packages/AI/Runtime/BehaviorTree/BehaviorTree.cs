using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Megumin.GameFramework.AI.BehaviorTree
{
    public class BehaviourTree
    {
        public virtual void Load() { }
        public void Tick()
        {

        }
    }

    public class MyTestBehaviourTree : BehaviourTree
    {
        public override void Load()
        {
            base.Load();
        }


    }
}

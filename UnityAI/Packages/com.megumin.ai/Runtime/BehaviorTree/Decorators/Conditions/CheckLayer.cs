using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Megumin.GameFramework.AI.BehaviorTree
{
    public class CheckLayer : ConditionDecorator<GameObject>
    {
        public LayerMask LayerMask = -1;

        protected override bool OnCheckCondition(object options = null)
        {
            if ((1 << MyAgent.layer & LayerMask) != 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}

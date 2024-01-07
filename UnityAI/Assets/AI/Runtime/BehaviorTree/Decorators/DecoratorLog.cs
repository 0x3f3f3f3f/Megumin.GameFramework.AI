using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Megumin.GameFramework.AI.BehaviorTree
{
    internal class DecoratorLog:IPreDecirator
    {
        public void BeforeNodeEnter(BTNode bTNode)
        {
            Debug.Log(bTNode.GetType().Name);
        }
    }
}

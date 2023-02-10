using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Megumin.GameFramework.AI.BehaviorTree
{
    [Serializable]
    public class CheckBool : IConditionable
    {
        public bool A = false;
        public bool B = false;
        public bool Cal()
        {
            return true;
        }

        public bool Result { get; set; }
    }
}



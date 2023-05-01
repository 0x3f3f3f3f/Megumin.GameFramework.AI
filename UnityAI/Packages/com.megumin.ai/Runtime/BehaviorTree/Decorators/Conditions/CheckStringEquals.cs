using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Megumin.Binding;

namespace Megumin.GameFramework.AI.BehaviorTree
{
    public class CheckStringEquals : StringEqualsDecorator
    {
        public RefVar_String Left;
        public RefVar_String Right;

        public override string GetResult()
        {
            return Left;
        }

        public override string GetCompareTo()
        {
            return Right;
        }
    }
}




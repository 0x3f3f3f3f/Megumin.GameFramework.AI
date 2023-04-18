using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Megumin.Binding;

namespace Megumin.GameFramework.AI.BehaviorTree
{
    public class CheckInt : CompareDecorator<int>
    {
        public RefVar_Int Left;
        public RefVar_Int Right;

        public override int GetResult()
        {
            return Left;
        }

        public override int GetCompareTo()
        {
            return Right;
        }
    }

    public class CheckFloat : CompareDecorator<float>
    {
        public RefVar_Float Left;
        public RefVar_Float Right;

        public override float GetResult()
        {
            return Left;
        }

        public override float GetCompareTo()
        {
            return Right;
        }
    }
}



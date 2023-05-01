using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Megumin.Binding;

namespace Megumin.GameFramework.AI.BehaviorTree
{
    public sealed class CheckInt : CompareDecorator<int>
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

    public sealed class CheckFloat : CompareDecorator<float>
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

    public sealed class CheckString : CompareDecorator<string>
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



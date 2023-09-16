using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Megumin.Binding;

namespace Megumin.AI.BehaviorTree
{
    [Obsolete("use CompareInt_Decorator instead.", true)]
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

    [Obsolete("use CompareFloat_Decorator instead.", true)]
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

    [Obsolete("use CompareString_Decorator instead.", true)]
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



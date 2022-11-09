using System;
using System.Collections.Generic;
using System.Text;

namespace Megumin.GameFramework.AI
{
    public interface IAIRuntime
    {

    }

    public interface INode
    {
        string Name { get; }
        string Description { get; }
    }
}

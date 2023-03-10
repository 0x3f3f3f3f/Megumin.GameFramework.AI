using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Megumin.GameFramework.AI.BehaviorTree
{
    public interface IVariable
    {
        string Name { get; }
        object GetValue();
    }

    public interface IVariable<T>
    {
        T Value { get; set; }
    }

    public class TestVariable : IVariable
    {
        public string Name { get; set; }

        public object GetValue()
        {
            throw new NotImplementedException();
        }
    }

    [Flags]
    public enum ParamVariableMode
    {
        Mapping = 1 << 0,
        Direct = 1 << 1,
        MappingAndFallback = Mapping | Direct,
    }

    internal class ParamVariable<T> : TestVariable, IVariable<T>
    {
        public T Value { get; set; }
        public string Path;
        public ParamVariableMode Mode = ParamVariableMode.MappingAndFallback;
    }
}

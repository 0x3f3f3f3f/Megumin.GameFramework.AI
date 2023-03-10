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

    public class TestVariable: IVariable
    {
        public string Name { get; set; }

        public object GetValue()
        {
            throw new NotImplementedException();
        }
    }

    internal class ParamVariable<T>: IVariable<T>
    {
        public T Value { get; set; }
    }

    public class MappedVariable<T> : IVariable<T>
    {
        public string Path;
        public T Value { get; set; }
    }
}

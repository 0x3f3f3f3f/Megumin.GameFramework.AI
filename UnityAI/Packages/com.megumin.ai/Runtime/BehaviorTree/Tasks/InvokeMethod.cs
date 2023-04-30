using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Megumin.Binding;

namespace Megumin.GameFramework.AI.BehaviorTree
{
    public class InvokeMethod
    {
        public string path;
        public void Test()
        {
            if (BindingParser.Instance.TryCreateMethodDelegate<Action<int>>(path, this, out var methodDelegate))
            {

            }
        }
    }
}

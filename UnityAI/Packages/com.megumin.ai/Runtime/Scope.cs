using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Megumin.GameFramework.AI
{
    internal class Scope : IDisposable
    {
        public bool IsEnter;

        public Scope Enter()
        {
            IsEnter = true;
            return this;
        }

        public void Dispose()
        {
            IsEnter = false;
        }
    }
}

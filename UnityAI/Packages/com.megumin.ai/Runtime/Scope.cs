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
        public static implicit operator bool(Scope scope)
        {
            return scope.IsEnter;
        }

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

    /// <summary>
    /// 同名唯一,用于控制代码是否执行到某个特定域。
    /// 用于再某些代码执行时，禁用另一些代码的执行。
    /// </summary>
    internal class HashSetScope
    {
        public HashSet<string> Users = new HashSet<string>();
        public Handle Enter(string user = "default")
        {
            Handle handle = new(this, user);
            Users.Add(user);
            return handle;
        }

        public void Exit(string user)
        {
            Users.Remove(user);
        }

        public bool Value => Users.Count > 0;
        public static implicit operator bool(HashSetScope scope)
        {
            return scope.Value;
        }

        public string LogUsers
        {
            get
            {
                string str = "";
                foreach (var user in Users)
                {
                    str += $"    {user}";
                }
                return str;
            }
        }


        public class Handle : IDisposable
        {
            public Handle(HashSetScope scope, string user)
            {
                Scope = scope;
                User = user;
            }

            public HashSetScope Scope { get; }
            public string User { get; }

            public void Dispose()
            {
                Scope.Exit(User);
            }
        }
    }
}

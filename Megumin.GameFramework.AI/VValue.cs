using System;
using System.Collections.Generic;
using System.Text;

namespace Megumin.GameFramework.AI
{
    public class VValue
    {
        public bool UseTypeConvert = true;
    }

    public class VValue<T> : VValue
    {
        public T Value { get; set; }

        public object Agent { get; set; }

        public string BindNavigetion { get; set; }

        public Func<T> Getter { get; set; }
        public Action<T> Setter { get; set; }

        public void Init()
        {
            Type type= typeof(T);
            var p = type.GetProperty(BindNavigetion);
            if (p.PropertyType.IsSubclassOf(typeof(T)))
            {
                Getter = p.GetMethod.CreateDelegate<Func<T>>(Agent);
            }
            else
            {
                if (p.PropertyType == typeof(int))
                {
                    Getter = () =>
                    {
                        var v = (int)p.GetValue(Agent);
                        return ConvGetter(v);
                    };
                }
            }
        }

        public T ConvGetter(int s)
        {
            return default(T);
        }
    }

    public static class TypcConvert
    {
        public class C<F, T>
        {
            public Type From;
            public Type To;

            public T GetConv(F f)
            {
                return default(T);
            }

            public void SetConv(F f, T val)
            {
            }
        }
    }

    public class GlobalBlackboard
    {
        static object[] agents = new object[10];
        static Dictionary<string,object> keyValuePairs= new Dictionary<string,object>();

        public void Set()
        {

        }
    }

    public enum VScope
    {
        Export,
        Binding,
        Local,
        SubGraph,
        Graph,
    }
}

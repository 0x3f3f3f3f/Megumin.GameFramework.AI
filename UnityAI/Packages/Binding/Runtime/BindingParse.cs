using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor.Search;
using UnityEngine;

namespace Megumin.Binding
{
    [Flags]
    public enum BindResult
    {
        None = 0,
        Get = 1,
        Set = 2,
        Both = Get | Set,
    }

    public class BindingParse
    {
        public static readonly BindingParse Instance = new BindingParse();


    }

    public class UnityBindingParse
    {
        public (BindResult ParseResult, Func<T> Getter, Action<T> Setter)
            InitializeBinding<T>(string BindingString, object agent, object extnalObj)
        {
            BindResult ParseResult = BindResult.None;
            Func<T> Getter = null;
            Action<T> Setter = null;


            if (string.IsNullOrEmpty(BindingString))
            {

            }
            else
            {
                var path = BindingString.Split('/');
                GameObject gameObject = agent as GameObject;
                if (extnalObj is GameObject ex && ex)
                {
                    gameObject = ex;
                }

                var (instance, bindType) = GetBindInstanceAndType(path[0], gameObject);

                {
                    //尝试绑定propertyInfo
                    var propertyInfo = bindType.GetProperty(path[1]);
                    if (propertyInfo.CanRead)
                    {
                        var getMethod = propertyInfo.GetGetMethod();
                        var firstArgs = instance;
                        if (getMethod.IsStatic)
                        {
                            firstArgs = null;
                        }
                        Getter = (Func<T>)Delegate.CreateDelegate(typeof(Func<T>), firstArgs, getMethod);
                        ParseResult |= BindResult.Get;
                    }

                    if (propertyInfo.CanWrite)
                    {
                        var setMethod = propertyInfo.GetSetMethod();
                        var firstArgs = instance;
                        if (setMethod.IsStatic)
                        {
                            firstArgs = null;
                        }
                        Setter = (Action<T>)Delegate.CreateDelegate(typeof(Action<T>), firstArgs, setMethod);
                        ParseResult |= BindResult.Set;
                    }
                }

                {
                    //尝试绑定field
                    var fieldInfo = instance.GetType().GetField(path[1]);

                }
            }

            return (ParseResult, Getter, Setter);
        }

        private static (object Object, Type Type) GetBindInstanceAndType(string compName, GameObject gameObject)
        {
            if (compName == "UnityEngine.GameObject")
            {
                return (gameObject, typeof(UnityEngine.GameObject));
            }

            var type = GetCompnentType(compName);


            if (type == null)
            {
                type = CacheGetType(compName);
                return (gameObject, type);
            }
            else
            {
                var comp = gameObject.GetComponentInChildren(type);
                return (comp, type);
            }
        }



        static Dictionary<string, Type> cache = new Dictionary<string, Type>();
        private static Type CacheGetType(string typeFullName)
        {
            if (cache.TryGetValue(typeFullName, out var type))
            {
                return type;
            }

            var firsttype = Type.GetType(typeFullName);
            if (firsttype != null)
            {
                cache[firsttype.FullName] = firsttype;
                return firsttype;
            }

            var componentTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Where(t => typeof(UnityEngine.Object).IsAssignableFrom(t))
                .Where(t => t.IsPublic).ToList();

            componentTypes = componentTypes.OrderBy(t => t.Name).ToList();

            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblies)
            {
                foreach (var extype in assembly.GetTypes())
                {
                    if (cache.ContainsKey(extype.FullName))
                    {
                        //Debug.LogError($"{extype.FullName}");
                    }
                    cache[extype.FullName] = extype;
                }
            }

            //重新在找一次
            if (cache.TryGetValue(typeFullName, out type))
            {
                return type;
            }
            return null;
        }


        static Dictionary<string, Type> compcache = new Dictionary<string, Type>();
        public static Type GetCompnentType(string typeFullName)
        {
            if (compcache.TryGetValue(typeFullName, out var type))
            {
                return type;
            }

            var firsttype = Type.GetType(typeFullName);
            if (firsttype != null)
            {
                compcache[firsttype.FullName] = firsttype;
                return firsttype;
            }

            var componentTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Where(t => typeof(UnityEngine.Object).IsAssignableFrom(t))
                .Where(t => t.IsPublic).ToList();

            componentTypes = componentTypes.OrderBy(t => t.Name).ToList();

            foreach (var extype in componentTypes)
            {
                if (compcache.ContainsKey(extype.FullName))
                {
                   // Debug.LogError($"{extype.FullName}");
                }
                compcache[extype.FullName] = extype;
            }

            //重新在找一次
            if (compcache.TryGetValue(typeFullName, out type))
            {
                return type;
            }
            return null;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.Json;
using Newtonsoft.Json;
using System;
using System.Reflection;
using System.Linq;

namespace Megumin.Binding
{
    public interface IData
    {

    }

    public interface IData<T> : IData
    {
        T Value { get; set; }
    }

    /// <summary>
    /// 运行时值
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Bindable<T> : IData<T>
    {
        public T Value { get; set; }

        protected Func<T> Getter;
        protected Action<T> Setter;


        public static implicit operator T(Bindable<T> value)
        {
            return value.Value;
        }
    }

    /// <summary>
    /// 绑定信息序列化
    /// </summary>
    [Serializable]
    public class SerializeValue
    {
        public string Key;
        public bool IsBinding;
        public string BindingString;
        public string Value;


    }

    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class BindableIntValue
    {
        public string Key;
        public bool IsBinding;
        public string BindingString;
        public int DefaultValue;
        public GameObject extnalObj;
        public int xOffset = 0, yOffset = 0;

        BindResult ParseResult = BindResult.None;
        Func<int> Getter;
        Action<int> Setter;

        public int Value
        {
            get { return (Getter != null ? Getter() : DefaultValue); }
            set
            {
                if (Setter != null)
                {
                    Setter(value);
                }
                else
                {
                    DefaultValue = value;
                }
            }
        }

        public void InitializeBinding(object agent)
        {

            if (string.IsNullOrEmpty(BindingString))
            {

            }
            else
            {
                var path = BindingString.Split('/');
                GameObject gameObject = agent as GameObject;
                if (extnalObj)
                {
                    gameObject = extnalObj;
                }

                object instance = GetBindInstance(path[0], gameObject);

                {
                    //尝试绑定propertyInfo
                    var propertyInfo = instance.GetType().GetProperty(path[1]);
                    if (propertyInfo.CanRead)
                    {
                        var getMethod = propertyInfo.GetGetMethod();
                        Getter = (Func<int>)Delegate.CreateDelegate(typeof(Func<int>), instance, getMethod);
                        ParseResult |= BindResult.Get;
                    }

                    if (propertyInfo.CanWrite)
                    {
                        var setMethod = propertyInfo.GetSetMethod();
                        Setter = (Action<int>)Delegate.CreateDelegate(typeof(Action<int>), instance, setMethod);
                        ParseResult |= BindResult.Set;
                    }
                }

                {
                    //尝试绑定field
                    var fieldInfo = instance.GetType().GetField(path[1]);

                }
            }
        }

        private static object GetBindInstance(string compName, GameObject gameObject)
        {
            if (compName == "UnityEngine.GameObject")
            {
                return gameObject;
            }

            var type = GetCompnentType(compName);
            var comp = gameObject.GetComponentInChildren(type);
            return comp;
        }

        [Flags]
        public enum BindResult
        {
            None = 0,
            Get = 1,
            Set = 2,
            Both = Get | Set,
        }

        Dictionary<string, Type> cache = new Dictionary<string, Type>();
        private Type CacheGetType(string typeFullName)
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
                foreach (var extype in assembly.GetExportedTypes())
                {
                    if (cache.ContainsKey(extype.FullName))
                    {
                        Debug.LogError($"{extype.FullName}");
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
                    Debug.LogError($"{extype.FullName}");
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

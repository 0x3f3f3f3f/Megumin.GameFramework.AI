using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Megumin.Reflection;
using UnityEngine;
using UnityEngine.Profiling;

namespace Megumin.AI.BehaviorTree
{
    /// <summary>
    /// 防止尖峰帧卡顿，在合适的位置预热反射代码
    /// </summary>
    public static class PreventLagSpike
    {
        public static Task WarmUpAllAsync()
        {
            return Task.Run(() => { WarmUpAll(); });
        }

        //[UnityEngine.RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        public static void WarmUpAll()
        {
            Profiler.BeginSample("WarmUpAll");

            TypeCache.CacheAllTypes();

            WarmUpTypeCacheGenericType();

            //WarmUpNodeType();

            Profiler.EndSample();
        }

        public static void WarmUpNodeType()
        {
            var megumin_ai_behaviortree_assembly = typeof(BehaviorTree).Assembly;
            var types = megumin_ai_behaviortree_assembly.GetTypes();
            var baseType = typeof(BehaviorTreeElement);
            foreach (var type in types)
            {
                //很多生成的节点完全不必要预热。应该设置个白名单
                if (baseType.IsAssignableFrom(type))
                {
                    HotType(type);
                }
            }
        }

        public static List<Type> GenericType = new List<Type>()
        {
            typeof(int[]),
            typeof(List<int>),

            typeof(string[]),
            typeof(List<string>),

            typeof(Vector3[]),
            typeof(List<Vector3>),

            typeof(GameObject[]),
            typeof(List<GameObject>),

            typeof(Transform[]),
            typeof(List<Transform>),
        };

        /// <summary>
        /// 预热泛型，避免TryMakeGenericType
        /// </summary>
        public static void WarmUpTypeCacheGenericType()
        {
            foreach (var type in GenericType)
            {
                HotType(type);
            }
        }

        /// <summary>
        /// 预热一个类型
        /// </summary>
        /// <param name="type"></param>
        public static void HotType(Type type)
        {
            TypeCache.HotType(type);
            TypeCache.HotType(type.FullName.StripTypeName(), type);

            try
            {
                //预热反射创建
                Activator.CreateInstance(type);
            }
            catch (Exception)
            {

            }

            //预热反射成员
            var members = type.GetMembers();

            object[] attris = null;
            foreach (var item in members)
            {
                attris = item.GetCustomAttributes(true);
            }

            var fields = type.GetFields();
            var props = type.GetProperties();
            var methods = type.GetMethods();
        }
    }
}





using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Megumin.Binding;
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

        [UnityEngine.RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        public static void WarmUpAll()
        {
            Profiler.BeginSample("WarmUpAll");

            TypeCache.CacheAllTypes();

            WarmUpTypeCacheGenericType();

            WarmUpNodeType();

            foreach (var item in VariableCreator.AllCreator)
            {
                var refVar = item.Create();
                if (refVar != null)
                {
                    HotType(refVar.GetType());
                }
            }

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
                    var attri = type.GetCustomAttributes(typeof(CodeGeneratorInfoAttribute), false);
                    if (attri != null && attri.Length > 0)
                    {
                        //忽略生成的节点类型，这些类型太多了，不要预热
                        continue;
                    }
                    else
                    {
                        HotType(type);
                    }
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

            typeof(BTNode[]),
            typeof(List<BTNode>),

            typeof(IDecorator[]),
            typeof(List<IDecorator>),
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
            foreach (var item in fields)
            {
                attris = item.GetCustomAttributes(true);
            }

            var props = type.GetProperties();
            foreach (var item in props)
            {
                attris = item.GetCustomAttributes(true);
            }

            var methods = type.GetMethods();
            foreach (var item in methods)
            {
                attris = item.GetCustomAttributes(true);
            }
        }
    }
}





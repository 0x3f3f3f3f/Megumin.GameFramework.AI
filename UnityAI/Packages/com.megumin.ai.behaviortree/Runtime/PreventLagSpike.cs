using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Megumin.Binding;
using Megumin.Reflection;
using Megumin.Serialization;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.Serialization;

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

            TypeCache.CacheAllTypes();//200ms+

            WarmUpStaticMember();

            WarmUpTypeCacheGenericType(); //2ms~20ms

            WarmUpType();

            WarmUpNodeType();

            WarmUpRefVar();

            Profiler.EndSample();
        }

        private static void WarmUpType()
        {
            foreach (var type in WarmUpTypes)
            {
                HotType(type);
                WarmUpTypeReflection(type);
            }
        }

        /// <summary>
        /// 预热静态类型，初始化静态成员
        /// </summary>
        public static void WarmUpStaticMember()
        {
            StringFormatter.TryDeserialize("System.Boolean", "true", out var _); //0.5ms~1.5ms
            var rpn = TypeHelper.ReplacePartialName; //0.1ms+
            var adps = TypeAdpter.Adpters; //0.1ms+
        }

        private static void WarmUpRefVar()
        {
            foreach (var item in VariableCreator.AllCreator)
            {
                var refVar = item.Create();
                if (refVar != null)
                {
                    Type type = refVar.GetType();
                    HotType(type);
                    WarmUpTypeReflection(type);
                }
            }
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
                        WarmUpTypeReflection(type);
                    }
                }
            }
        }

        public static List<Type> WarmUpTypes = new List<Type>()
        {
            typeof(TreeMeta),
            typeof(NodeMeta),
        };

        public static List<Type> GenericTypes = new List<Type>()
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
            foreach (var type in GenericTypes)
            {
                HotType(type);
                WarmUpTypeReflection(type);
            }
        }

        /// <summary>
        /// TypeCache预热一个类型
        /// </summary>
        /// <param name="type"></param>
        public static void HotType(Type type)
        {
            if (type == null)
            {
                return;
            }

            TypeCache.HotType(type);
            TypeCache.HotType(type.FullName.StripTypeName(), type);
        }

        /// <summary>
        /// 预热一个类型反射成员
        /// </summary>
        /// <param name="type"></param>
        public static void WarmUpTypeReflection(Type type)
        {
            if (type == null)
            {
                return;
            }

            

            try
            {
                //预热反射创建
                Activator.CreateInstance(type);
            }
            catch (Exception)
            {

            }

            type.WarmUpReflection_TrySetMember();
        }
    }
}





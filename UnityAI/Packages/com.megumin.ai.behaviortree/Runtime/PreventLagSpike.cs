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
    /// 可以在合适的位置调用PreventLagSpike.WarmUpAll。  
    /// 也可以打开RuntimeInitializeOnLoadMethod特性，这样程序启动时自动预热。  
    /// 使用多线程预热更合适，但是无法确定预热完成时间。  
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

        public static void WarmUpType()
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

        public static void WarmUpRefVar()
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

            //List<Type> types2 = new List<Type>();

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
                        //types2.Add(type);
                        HotType(type);
                        WarmUpTypeReflection(type);
                    }
                }
            }

            //types2.Clear();
        }

        public static List<Type> WarmUpTypes = new List<Type>()
        {
            typeof(TreeMeta),
            typeof(NodeMeta),
            typeof(RefVar_Transform),
            typeof(RefVar_GameObject),
            typeof(Destination),
            typeof(LogInfo),
            typeof(GameObjectFilter),
        };

        public static List<Type> GenericTypes = new List<Type>()
        {
            typeof(bool),
            typeof(int),
            typeof(float),
            typeof(double),
            typeof(string),

            typeof(Vector2),
            typeof(Vector3),
            typeof(Vector4),
            typeof(Quaternion),
            typeof(Color),
            typeof(LayerMask),

            typeof(TagMask),
            typeof(GameObject),
            typeof(Transform),

            typeof(BTNode),
            typeof(IDecorator),
        };

        /// <summary>
        /// 预热泛型，避免TryMakeGenericType
        /// </summary>
        public static void WarmUpTypeCacheGenericType()
        {
            var enableGeneric = typeof(Enable<>);
            var listGeneric = typeof(List<>);
            var refVarGeneric = typeof(RefVar<>);

            foreach (var type in GenericTypes)
            {
                HotType(type);
                WarmUpTypeReflection(type);

                {
                    //Enable
                    var enable = enableGeneric.MakeGenericType(type);
                    HotType(enable);
                    WarmUpTypeReflection(enable);
                }

                {
                    //RefVar
                    var refvar = refVarGeneric.MakeGenericType(type);
                    HotType(refvar);
                    WarmUpTypeReflection(refvar);
                }

                {
                    //Array
                    var array = type.MakeArrayType();
                    HotType(array);
                    WarmUpTypeReflection(array);

                    //Enable Array
                    var enablearray = enableGeneric.MakeGenericType(array);
                    HotType(enablearray);
                    WarmUpTypeReflection(enablearray);
                }

                {
                    //List
                    var list = listGeneric.MakeGenericType(type);
                    HotType(list);
                    WarmUpTypeReflection(list);

                    //Enable List
                    var enablelist = enableGeneric.MakeGenericType(list);
                    HotType(enablelist);
                    WarmUpTypeReflection(enablelist);
                }
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





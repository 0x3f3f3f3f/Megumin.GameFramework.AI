using System;
using System.Collections;
using System.Collections.Generic;
using Megumin.Reflection;
using UnityEngine;

namespace Megumin.Serialization
{

    public class RefFinder : IRefFinder
    {
        public Dictionary<string, object> RefDic { get; } = new();
        public RefFinder() { }

        public IRefFinder Parent { get; set; }

        public IRefFinder Fallback { get; set; }
        //public IRefFinder BubbleFallback { get; set; }

        //手动GlobalFallback 赋值给 Fallback，否则会逻辑错误。
        //存在父时，父查找失败会直接查找GlobalFallback，而是先查找子再查找global。
        //public static IRefFinder GlobalFallback { get; set; }

        public bool TryGetRefValue(string refName, out object refValue)
        {
            if (Parent != null && Parent.TryGetRefValue(refName, out refValue))
            {
                return true;
            }

            if (RefDic.TryGetValue(refName, out refValue))
            {
                return true;
            }

            if (Fallback != null && Fallback.TryGetRefValue(refName, out refValue))
            {
                return true;
            }

            //if (BubbleFallback != null && BubbleFallback.TryGetRefValue(refName, out refValue))
            //{
            //    return true;
            //}

            //if (GlobalFallback != null && GlobalFallback.TryGetRefValue(refName, out refValue))
            //{
            //    return true;
            //}

            refValue = null;
            return false;
        }
    }

    [Serializable]
    public class UnityObjectData : SerializationData
    {
        public UnityEngine.Object Ref;
    }

    /// <summary>
    /// 没有子成员的序列化数据，不包含嵌套的
    /// SerializationData解决不了同时支持List<Ref<>> 和Ref<List<>>问题。会导致循环嵌套。需要重新设计。
    /// </summary>
    [Serializable]
    public class BasicData : SerializationData
    {
        public string Type;
        public string Value;
    }

    [Serializable]
    public class ObjectData : SerializationData, IComparable<ObjectData>
    {
        public string Type;
        public List<BasicData> Member;

        public const string NullType = "$null";
        public const string RefType = "$ref";

        public bool TrySerialize(string objectRefName,
                                 object value,
                                 Stack<(string name, object value)> needSerialization,
                                 List<UnityObjectData> objRefs,
                                 Dictionary<object, string> cacheRef,
                                 GetSerializeMembers<object> getSerializeMembers = null)
        {
            Name = objectRefName;

            if (value == null)
            {
                Type = NullType;
                return true;
            }

            var type = value.GetType();
            Type = type.FullName;

            bool TrySerializeMember(string memberName,
                object memberValue,
                Type memberType,
                out BasicData basic)
            {
                basic = new();

                basic.Name = memberName;

                if (typeof(UnityEngine.Object).IsAssignableFrom(memberType))
                {
                    basic.Type = RefType;
                    var refName = $"{objectRefName}.{memberName}";
                    basic.Value = refName;

                    UnityObjectData unityObjRef = new();
                    unityObjRef.Name = refName;
                    unityObjRef.Ref = memberValue as UnityEngine.Object;
                    objRefs.Add(unityObjRef);
                }
                else if (memberValue == null)
                {
                    basic.Type = NullType;
                    basic.Value = null;
                }
                else
                {
                    var memberValueType = memberValue.GetType();
                    basic.Type = memberValueType.FullName;
                    if (StringFormatter.TrySerialize(memberValue, out var destination))
                    {
                        basic.Value = destination;
                    }
                    else
                    {
                        //当作引用序列化
                        if (!cacheRef.TryGetValue(memberValue, out var refName))
                        {
                            //当前还没有缓存这个引用对象
                            refName = $"{objectRefName}.{memberName}";
                            needSerialization.Push((refName, memberValue));
                        }

                        basic.Type = RefType;
                        basic.Value = refName;
                        //if (memberValueType.IsClass)
                        //{

                        //}
                        //else
                        //{
                        //    Debug.LogError($"objectRefName.{basic.Name} {basic.Type}序列化失败");
                        //    continue;
                        //}
                    }
                }

                return true;
            }

            List<BasicData> ms = new();

            if (getSerializeMembers != null)
            {
                foreach (var (memberName, memberValue, memberType) in getSerializeMembers.Invoke(value))
                {
                    if (TrySerializeMember(memberName, memberValue, memberType, out var basic))
                    {
                        ms.Add(basic);
                    }
                }
            }
            else
            {
                foreach (var (memberName, memberValue, memberType) in value.GetSerializeMembers())
                {
                    if (TrySerializeMember(memberName, memberValue, memberType, out var basic))
                    {
                        ms.Add(basic);
                    }
                }
            }

            if (ms.Count > 0)
            {
                Member = ms;
            }

            Type = Type.StripTypeName();
            return true;
        }

        /// <summary>
        /// 仅根据类型反射创建对象实例
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TryCreateInstance(out object value)
        {
            if (Type == NullType)
            {
                value = null;
                return true;
            }

            if (TypeCache.TryGetType(Type, out var type))
            {
                try
                {
                    if (type.IsArray)
                    {
                        //数组和集合创建实例时要附带容量
                        var count = Member?.Count ?? 0;
                        value = Activator.CreateInstance(type, new object[] { count });
                        return true;
                    }
                    //这里不要带容量了，List用Insert方式添加
                    //else if (type.IsGenericType)
                    //{
                    //    //泛型集合
                    //    if (type.GetGenericTypeDefinition() == typeof(List<>))
                    //    {
                    //        //数组和集合创建实例时要附带容量
                    //        var count = Member?.Count ?? 0;
                    //        value = Activator.CreateInstance(type, new object[] { count });
                    //        return true;
                    //    }
                    //    else if (type.GetGenericTypeDefinition() == typeof(Dictionary<,>))
                    //    {
                    //        //数组和集合创建实例时要附带容量
                    //        var count = Member?.Count ?? 0;
                    //        value = Activator.CreateInstance(type, new object[] { count });
                    //        return true;
                    //    }
                    //}

                    value = Activator.CreateInstance(type);
                    return true;
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
            }

            value = default;
            return false;
        }

        /// <summary>
        /// 将成员反序列到指定实例中
        /// </summary>
        /// <param name="value"></param>
        /// <param name="refFinder"></param>
        /// <returns></returns>
        public bool TryDeserialize(object value, IRefFinder refFinder = null)
        {
            if (Type == NullType)
            {
                return true;
            }

            if (value == null)
            {
                return false;
            }

            bool TryDeserializeMember(BasicData data,
                out object memberValue)
            {
                if (data.Type == NullType)
                {
                    memberValue = null;
                    return true;
                }
                else if (data.Type == RefType)
                {
                    if (refFinder == null)
                    {
                        memberValue = null;
                        return true;
                    }

                    return refFinder.TryGetRefValue(data.Value, out memberValue);
                }
                else
                {
                    return StringFormatter.TryDeserialize(data.Type, data.Value, out memberValue);
                }
            }

            if (Member != null)
            {
                if (value is IDictionary dictionary)
                {
                    Debug.LogError($"不支持字典");
                    return false;
                }
                else if (value is Array array)
                {
                    Type memberType = array.GetType().GetElementType();

                    if (memberType == null)
                    {
                        Debug.LogError($"找不到特化类型");
                        return false;
                    }

                    for (int i = 0; i < Member.Count; i++)
                    {
                        var memberData = Member[i];
                        if (TryDeserializeMember(memberData, out var memberValue))
                        {
                            if (memberValue is UnityEngine.Object uobj && !uobj && memberType != memberValue.GetType())
                            {
                                //空的UnityEngine.Object 反序列化后丢失真实类型。
                                //需要改为null，否则SetValue时会导致类型不匹配异常
                                memberValue = null;
                            }

                            array.SetValue(memberValue, i);
                        }
                    }
                }
                else if (value is IList list)
                {
                    Type memberType = list.GetType().GetGenericArguments()?[0];

                    if (memberType == null)
                    {
                        Debug.LogError($"找不到特化类型");
                        return false;
                    }

                    for (int i = 0; i < Member.Count; i++)
                    {
                        var memberData = Member[i];
                        if (TryDeserializeMember(memberData, out var memberValue))
                        {
                            if (memberValue is UnityEngine.Object uobj && !uobj && memberType != memberValue.GetType())
                            {
                                //空的UnityEngine.Object 反序列化后丢失真实类型。
                                //需要改为null，否则SetValue时会导致类型不匹配异常
                                memberValue = null;
                            }

                            //Todo? 这里会不会导致乱序？
                            list.Insert(i, memberValue);
                        }
                    }
                }
                else
                {
                    foreach (var memberData in Member)
                    {
                        if (TryDeserializeMember(memberData, out var memberValue))
                        {
                            value.TrySetMemberValue(memberData.Name, memberValue);
                        }
                    }
                }
            }

            return true;
        }

        public int CompareTo(ObjectData other)
        {
            return Name.CompareTo(other.Name);
        }

        public bool TryInstantiate<T>(out T value, IRefFinder refFinder = null)
        {
            if (TryCreateInstance(out var temp) && temp is T tValue)
            {
                if (TryDeserialize(temp, refFinder))
                {
                    value = tValue;
                    return true;
                }
            }

            value = default;
            return false;
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Megumin.Serialization
{
    /// <summary>
    /// SerializationData解决不了同时支持List<Ref<>> 和Ref<List<>>问题。会导致循环嵌套。需要重新设计。
    /// </summary>
    [Serializable]
    public class SerializationData2
    {

        public class Basic
        {
            public string Data;
            public UnityEngine.Object refObject;
            /// <summary>
            /// 必须保存类型名，用于处理多态？ 
            /// 简单类型不存在基础关系？直接根据成员类型获取？
            /// </summary>
            public string T;
        }

        public string Data;
        public List<UnityEngine.Object> refObject = new();

        public bool TrySerialize(string name, object value)
        {

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

        const string NullType = "$null";
        const string RefType = "$ref";

        public bool TrySerialize(string objectRefName,
                                 object value,
                                 Stack<(string name, object value)> needS,
                                 List<UnityObjectData> objRefs,
                                 Dictionary<object, string> cahce)
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
                        if (!cahce.TryGetValue(memberValue, out var refName))
                        {
                            //当前还没有缓存这个引用对象
                            refName = $"{objectRefName}.{memberName}";
                            needS.Push((refName, memberValue));
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
            if (value is IDictionary dictionary)
            {
                Debug.LogError($"不支持字典");
                return false;
            }
            else if (value is IList list)
            {
                var index = 0;
                var memberType = type.GetGenericArguments()[0];
                foreach (var item in list)
                {
                    if (TrySerializeMember($"Element{index}", item, memberType, out var basic))
                    {
                        ms.Add(basic);
                    }
                    index++;
                }
                return true;
            }
            else
            {
                var members = type.GetFields();
                foreach (var fieldInfo in members)
                {
                    var memberValue = fieldInfo.GetValue(value);
                    if (TrySerializeMember(fieldInfo.Name, memberValue, fieldInfo.FieldType, out var basic))
                    {
                        ms.Add(basic);
                    }
                }

                //MemberFilter memberFilter = null;
                //foreach (var (memberName, memberValue, memberType) in memberFilter.Invoke(value))
                //{
                //    if (TrySerializeMember(memberName, memberValue, memberType, out var basic))
                //    {
                //        ms.Add(basic);
                //    }
                //}
            }

            if (ms.Count > 0)
            {
                Member = ms;
            }
            return true;
        }

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

        public bool TryDeserialize(object value, Dictionary<string, object> cache)
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
                    return cache.TryGetValue(data.Value, out memberValue);
                }
                else
                {
                    return StringFormatter.TryDeserialize(data.Type, data.Value, out memberValue);
                }
            }

            foreach (var memberData in Member)
            {
                if (TryDeserializeMember(memberData, out var memberValue))
                {
                    var fieldInfo = value.GetType().GetField(memberData.Name);
                    fieldInfo.SetValue(value, memberValue);
                }
            }

            return true;
        }

        /// <summary>
        /// 序列化哪些成员，增加一个回调函数
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public delegate IEnumerable<(string MemberName, object MemberValue, Type MemberType)>
            MemberFilter(object value);

        public virtual IEnumerable<(string MemberName, object MemberValue, Type MemberType)>
            GetSerializeMember(object value)
        {
            yield break;
        }

        public int CompareTo(ObjectData other)
        {
            return Name.CompareTo(other.Name);
        }
    }
}

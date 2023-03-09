using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Megumin.Serialization;
using UnityEngine;

namespace Megumin.GameFramework.AI.Serialization
{
    public interface IParameterData
    {
        bool Instantiate(object instance);
    }

    public abstract class ParameterData : IParameterData
    {
        public string MemberName;

        public abstract bool Instantiate(object instance);
    }

    [Serializable]
    public abstract class GenericParameterData<T> : ParameterData, IParameterData
    {

        public T Value;
        public virtual bool TryDeserialize(out T value)
        {
            value = Value;
            return true;
        }

        public override bool Instantiate(object instance)
        {
            if (instance == null)
            {
                return false;
            }

            //Todo: 要不要使用TokenID查找
            var member = instance.GetType().GetMember(MemberName).FirstOrDefault();
            if (member != null && TryDeserialize(out var value))
            {
                if (member is FieldInfo fieldInfo)
                {
                    fieldInfo.SetValue(instance, value);
                    return true;
                }
                else if (member is PropertyInfo propertyInfo)
                {
                    propertyInfo.SetValue(instance, value);
                    return true;
                }
            }

            return false;
        }
    }

    [Flags]
    public enum ParameterDataType
    {
        // 0-3
        None = 0,
        IsClass = 1 << 0,
        IsNull = 1 << 1,

        //4-15
        IsPrimitive = 1 << 4,
        IsEnum = 1 << 5,
        IsString = 1 << 6,
        IsBasicType = 1 << 7,
        IsUnityObject = 1 << 8,
        /// <summary>
        /// Vector2,Vector3
        /// </summary>
        IsUnityBasicType = 1 << 9,

        //16-23
        IsCollection = 1 << 16,
        IsArray = 1 << 17,
        IsList = 1 << 18,
        IsDictionary = 1 << 19,

        //24-31
        IsBinary = 1 << 24,
        IsJson = 1 << 25,
        IsXML = 1 << 26,
    }

    /// <summary>
    /// TODO： 拆分可包含循环和不可包含循环的data，解决Serialization depth limit 10 exceeded 问题
    /// </summary>
    [Serializable]
    public class CustomParameterData : ParameterData
    {
        /// <summary>
        /// <para/>https://learn.microsoft.com/zh-cn/dotnet/api/system.type.gettype?view=netframework-4.7.1#system-type-gettype(system-string)
        /// <para/>https://stackoverflow.com/questions/61698509/returns-null-when-executing-type-gettypesystem-collections-generic-sorteddicti
        /// <para/>应不应该使用 Type.AssemblyQualifiedName？1.太长，浪费空间 2，打包后运行时程序集有没有可能不一致
        /// <para/>自行反射获取类型？目前采用的方案
        /// </summary>
        public string TypeName;
        public string Value;
        public UnityEngine.Object RefObject;
        public List<CustomParameterData> Collection;
        public ParameterDataType DataType = ParameterDataType.None;

        /// <summary>
        /// https://blog.unity.com/technology/serialization-in-unity
        /// </summary>
        /// <param name="member"></param>
        /// <param name="instance"></param>
        /// <param name="defualtValueInstance"></param>
        /// <returns></returns>
        public bool TrySerialize(MemberInfo member, object instance, object defualtValueInstance)
        {
            Debug.Log(member);

            object memberValue = null;
            object defaultMemberValue = null;

            if (member is FieldInfo field)
            {
                memberValue = field.GetValue(instance);
                defaultMemberValue = field.GetValue(defualtValueInstance);
            }
            else if (member is PropertyInfo property)
            {
                memberValue = property.GetValue(instance);
                defaultMemberValue = property.GetValue(defualtValueInstance);
            }

            //都是null认为不需要保存
            var nullEqual = memberValue == null && defaultMemberValue == null;

            if (nullEqual || memberValue.Equals(defaultMemberValue))
            {
                Debug.Log($"值为初始值或者默认值没必要保存");
            }
            else
            {
                return TrySerialize(member.Name, memberValue);
            }

            return false;
        }

        internal bool TrySerialize(string memberName, object value)
        {
            var valueActualType = value?.GetType();
            //if (valueActualType == typeof(int))
            //{
            //    IntParameterData this = new();
            //    this.MemberName = member.Name;
            //    this.Value = (int)ilist;
            //    return this;
            //}
            //else if (valueActualType == typeof(UnityEngine.Object))
            //{
            //    UnityEngineObjectParameterData this = new();
            //    this.MemberName = member.Name;
            //    this.Value = (UnityEngine.Object)ilist;
            //    return this;
            //}
            //else
            {
                MemberName = memberName;
                if (value == null)
                {
                    //引用类型并且值为null
                    DataType |= ParameterDataType.IsNull;
                }
                else
                {
                    TypeName = valueActualType?.FullName;
                    if (valueActualType.IsClass)
                    {
                        DataType |= ParameterDataType.IsClass;
                    }

                    if (valueActualType.IsGenericType)
                    {
                        //泛型集合
                        if (valueActualType.GetGenericTypeDefinition() == typeof(List<>))
                        {
                            var specializationType = valueActualType.GetGenericArguments()[0];
                            Debug.LogError($"List: {specializationType.Name}");

                            DataType |= ParameterDataType.IsList;
                            SerializeIList(value);
                        }
                        else if (valueActualType.GetGenericTypeDefinition() == typeof(Dictionary<,>))
                        {
                            var specializationKeyType = valueActualType.GetGenericArguments()[0];
                            var specializationValueType = valueActualType.GetGenericArguments()[1];
                            Debug.LogError($"Dictionary: {specializationKeyType.Name}----{specializationValueType.Name}");

                            DataType |= ParameterDataType.IsDictionary;
                            return false;
                        }
                        else
                        {
                            Debug.LogError($"GenericType: {valueActualType.Name}");
                            return false;
                        }
                    }
                    else if (valueActualType.IsArray)
                    {
                        //数组
                        var specializationType = valueActualType.GetElementType();

                        AssemblyName assemblyName = valueActualType.Assembly.GetName();
                        var testName = $"{valueActualType?.FullName},{assemblyName.Name}";
                        var resultType = Type.GetType(testName);
                        Debug.LogError($"Array: {specializationType.Name}----{testName}");
                        DataType |= ParameterDataType.IsArray;
                        SerializeIList(value);
                    }
                    else if (typeof(UnityEngine.Object).IsAssignableFrom(valueActualType))
                    {
                        DataType |= ParameterDataType.IsUnityObject;
                        RefObject = (UnityEngine.Object)value;
                    }
                    else if (valueActualType == typeof(string))
                    {
                        Value = (string)value;
                        DataType |= ParameterDataType.IsString;
                    }
                    else
                    {
                        //这里一定要取值得真实类型，解决多态序列化
                        if (Formater.TryGet(valueActualType, out var iformater))
                        {
                            Value = iformater.Serialize(value);
                        }
                        else
                        {
                            Debug.LogError($"{valueActualType.Name}    {memberName} 没找到Iformater");
                        }
                    }
                }
                return true;
            }
        }

        public bool SerializeIList(object ilist)
        {
            if (ilist is IList list && list.Count > 0)
            {
                List<CustomParameterData> dataList = new();

                var index = 0;
                foreach (var item in list)
                {
                    CustomParameterData elementData = new();
                    if (elementData.TrySerialize($"Element{index}", item))
                    {
                        dataList.Add(elementData);
                        index++;
                    }
                }

                if (dataList.Count > 0)
                {
                    Collection = dataList;
                }
            }

            return true;
        }

        public bool TryDeserialize(out object value)
        {
            if (DataType.HasFlag(ParameterDataType.IsClass) && DataType.HasFlag(ParameterDataType.IsNull))
            {
                value = null;
                return true;
            }

            if (DataType.HasFlag(ParameterDataType.IsList))
            {
                if (Collection == null)
                {
                    value = null;
                    return true;
                }
                else
                {
                    Type listType = Type.GetType(TypeName);
                    var list = TryCreateInstance(listType) as IList;

                    if (list != null)
                    {
                        foreach (var item in Collection)
                        {
                            if (item.TryDeserialize(out var elementValue))
                            {
                                list.Add(elementValue);
                            }
                        }
                        value = list;
                        return true;
                    }
                    else
                    {
                        value = null;
                        return false;
                    }
                }
            }

            if (DataType.HasFlag(ParameterDataType.IsArray))
            {
                if (Collection == null)
                {
                    value = null;
                    return true;
                }
                else
                {
                    //GameObject[] 类型取不到
                    Type arrayType = Type.GetType(TypeName);
                    Type elementType = null;
                    if (arrayType == null && TypeName.Length > 2)
                    {
                        string elementTypeFullName = TypeName.Substring(0, TypeName.Length - 2);
                        elementType = Type.GetType(elementTypeFullName);
                        if (elementType == null)
                        {
                            elementType = TypeCache.GetType(elementTypeFullName);
                        }
                    }
                    else
                    {
                        elementType = arrayType?.GetElementType();
                    }

                    if (elementType == null)
                    {
                        value = null;
                        return false;
                    }

                    var array = Array.CreateInstance(elementType, Collection.Count) as Array;
                    for (int i = 0; i < Collection.Count; i++)
                    {
                        var item = Collection[i];
                        if (item.TryDeserialize(out var elementValue))
                        {
                            array.SetValue(elementValue, i);
                        }
                    }

                    value = array;
                    return true;
                }
            }

            if (DataType.HasFlag(ParameterDataType.IsUnityObject))
            {
                value = RefObject;
                return true;
            }

            if (DataType.HasFlag(ParameterDataType.IsString))
            {
                value = Value;
                return true;
            }

            if (Formater.TryGet(TypeName, out var iformater))
            {
                return iformater.TryDeserialize(Value, out value);
            }
            value = default;
            return false;
        }

        public static object TryCreateInstance(Type cType)
        {
            try
            {
                return Activator.CreateInstance(cType);
            }
            catch (Exception)
            {

            }
            return null;
        }

        public override bool Instantiate(object instance)
        {
            if (instance == null)
            {
                return false;
            }

            //Todo: 要不要使用TokenID查找
            var member = instance.GetType().GetMember(MemberName)?.FirstOrDefault();
            if (member != null && TryDeserialize(out var value))
            {
                if (member is FieldInfo fieldInfo)
                {
                    fieldInfo.SetValue(instance, value);
                    return true;
                }
                else if (member is PropertyInfo propertyInfo)
                {
                    propertyInfo.SetValue(instance, value);
                    return true;
                }
            }

            return false;
        }
    }


}

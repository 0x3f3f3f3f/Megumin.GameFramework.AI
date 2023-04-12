using System;
using System.Collections;
using System.Collections.Generic;
using Megumin.Reflection;
using UnityEngine;

namespace Megumin.Serialization
{
    public interface ISerializationData
    {

    }

    public abstract class SerializationData : ISerializationData
    {
        public string Name;
    }

    [Serializable]
    public abstract class GenericSerializationData<T> : SerializationData, ISerializationData
    {
        public T Value;
        public virtual bool TryDeserialize(out T value)
        {
            value = Value;
            return true;
        }
    }

    [Flags]
    public enum SerializationDataType
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
        IsVariable = 1 << 10,
        /// <summary>
        /// 用户回调和框架默认模式只能二选一。
        /// 如果用户回调都都失败，框架默认模式也大概率失败，没有调用的意义。
        /// </summary>
        IsUserCallbackFormatter = 1 << 11,

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
    /// 拆分可包含循环和不可包含循环的data，解决Serialization depth limit 10 exceeded 问题
    /// </summary>
    [Serializable]
    public class BasicTypeSerializationData : SerializationData
    {
        /// <summary>
        /// <para/>https://learn.microsoft.com/zh-cn/dotnet/api/system.type.gettype?view=netframework-4.7.1#system-type-gettype(system-string)
        /// <para/>https://stackoverflow.com/questions/61698509/returns-null-when-executing-type-gettypesystem-collections-generic-sorteddicti
        /// <para/>应不应该使用 Type.AssemblyQualifiedName？1.太长，浪费空间 2，打包后运行时程序集有没有可能不一致
        /// <para/>自行反射获取类型？目前采用的方案
        /// </summary>
        public string TypeName;
        public string Data;
        public UnityEngine.Object RefObject;
        public SerializationDataType DataType = SerializationDataType.None;

        /// <summary>
        /// https://blog.unity.com/technology/serialization-in-unity
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TrySerialize(string name, object value)
        {
            Name = name;
            if (value == null)
            {
                //引用类型并且值为null
                DataType |= SerializationDataType.IsNull;
                Data = null;
                return true;
            }
            else
            {
                return TrySerializeNotNull(name, value);
            }
        }

        public virtual bool TrySerializeNotNull(string name, object value)
        {
            //用户回调和框架默认模式只能二选一
            //如果用户回调都都失败，框架默认模式也大概率失败，没有调用的意义。
            if (value is ICallbackFormatter<BasicTypeSerializationData> callbackFormatter)
            {
                DataType |= SerializationDataType.IsUserCallbackFormatter;
                if (callbackFormatter.TrySerialize(this))
                {
                    return true;
                }
            }
            else
            {
                var valueActualType = value.GetType();
                TypeName = valueActualType.FullName;
                if (valueActualType.IsClass)
                {
                    DataType |= SerializationDataType.IsClass;
                }

                return TrySerializeByType(valueActualType, name, value);
            }

            return false;
        }

        protected virtual bool TrySerializeByType(Type valueActualType, string name, object value)
        {
            if (typeof(UnityEngine.Object).IsAssignableFrom(valueActualType))
            {
                DataType |= SerializationDataType.IsUnityObject;
                RefObject = (UnityEngine.Object)value;
            }
            else if (valueActualType == typeof(string))
            {
                Data = (string)value;
                DataType |= SerializationDataType.IsString;
            }
            else
            {
                //这里一定要取值得真实类型，解决多态序列化
                if (StringFormatter.TrySerialize(valueActualType, out var destination))
                {
                    Data = destination;
                }
                else
                {
                    Debug.LogError($"{valueActualType.Name}    {name} 没找到Iformater");
                    return false;
                }
            }

            return true;
        }

        public bool TryDeserialize(out object value)
        {
            if ((DataType & SerializationDataType.IsNull) != 0
                && (DataType & SerializationDataType.IsClass) != 0)
            {
                value = null;
                return true;
            }

            return TryDeserializeNotNull(out value);
        }

        public virtual bool TryDeserializeNotNull(out object value)
        {
            //用户回调和框架默认模式只能二选一
            //如果用户回调都都失败，框架默认模式也大概率失败，没有调用的意义。
            if ((DataType & SerializationDataType.IsUserCallbackFormatter) != 0)
            {
                if (TryCreateInstance<ICallbackFormatter<BasicTypeSerializationData>>(out var formatter))
                {
                    if (formatter.TryDeserialize(this))
                    {
                        value = formatter;
                        return true;
                    }
                }
            }
            else
            {
                if ((DataType & SerializationDataType.IsUnityObject) != 0)
                {
                    if (RefObject)
                    {
                        value = RefObject;
                        return true;
                    }
                    else
                    {
                        //unity null
                        var dateType = TypeCache.GetType(TypeName);
                        var refObjetType = RefObject?.GetType();
                        if (dateType.IsAssignableFrom(refObjetType))
                        {
                            //没有重新打开编辑器时，销毁的对象仍然有真实类型，missRrefrence等情况
                            value = RefObject;
                            return true;
                        }
                        else
                        {
                            //重新打开编辑器时，无法准确获取真实类型，RefObject变为UnityEngine.Object 类型，
                            //需要返回null，否则SetValue时会导致类型不匹配异常
                            value = null;
                            return true;
                        }
                    }
                }

                if ((DataType & SerializationDataType.IsString) != 0)
                {
                    value = Data;
                    return true;
                }

                if (StringFormatter.TryDeserialize(TypeName, Data, out value))
                {
                    return true;
                }
            }

            value = default;
            return false;
        }

        public virtual bool TryCreateInstance<T>(out T insance)
        {
            try
            {
                var dateType = TypeCache.GetType(TypeName);
                var instance = Activator.CreateInstance(dateType);
                if (instance is T result)
                {
                    insance = result;
                    return true;
                }
            }
            catch (Exception)
            {

            }
            insance = default;
            return false;
        }
    }

    /// <summary>
    /// 设计缺陷无法处理循环嵌套。
    /// 应该使用展开引用的序列化方式
    /// </summary>
    [Serializable]
    public class CollectionSerializationData : BasicTypeSerializationData
    {
        public List<BasicTypeSerializationData> Collection;

        protected bool SerializeIList(object ilist)
        {
            if (ilist is IList list && list.Count > 0)
            {
                List<BasicTypeSerializationData> dataList = new();

                var index = 0;
                foreach (var item in list)
                {
                    BasicTypeSerializationData elementData = new();
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

        protected override bool TrySerializeByType(Type valueActualType, string memberName, object value)
        {
            if (valueActualType.IsGenericType)
            {
                //泛型集合
                if (valueActualType.GetGenericTypeDefinition() == typeof(List<>))
                {
                    //var specializationType = valueActualType.GetGenericArguments()[0];
                    //Debug.LogError($"List: {specializationType.Name}");

                    DataType |= SerializationDataType.IsList;
                    return SerializeIList(value);
                }
                else if (valueActualType.GetGenericTypeDefinition() == typeof(Dictionary<,>))
                {
                    //var specializationKeyType = valueActualType.GetGenericArguments()[0];
                    //var specializationValueType = valueActualType.GetGenericArguments()[1];
                    //Debug.LogError($"Dictionary: {specializationKeyType.Name}----{specializationValueType.Name}");

                    DataType |= SerializationDataType.IsDictionary;
                    return false;
                }
            }
            else if (valueActualType.IsArray)
            {
                //数组
                //var specializationType = valueActualType.GetElementType();

                //AssemblyName assemblyName = valueActualType.Assembly.GetName();
                //var testName = $"{valueActualType?.FullName},{assemblyName.Name}";
                //var resultType = Type.GetType(testName);
                //Debug.LogError($"Array: {specializationType.Name}----{testName}");

                DataType |= SerializationDataType.IsArray;
                return SerializeIList(value);
            }

            //非集合类型
            return base.TrySerializeByType(valueActualType, memberName, value);
        }

        public override bool TryDeserializeNotNull(out object value)
        {
            if ((DataType & SerializationDataType.IsList) != 0)
            {
                if (Collection == null)
                {
                    value = null;
                    return true;
                }
                else
                {
                    if (TryCreateInstance<IList>(out var list))
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

            if ((DataType & SerializationDataType.IsArray) != 0)
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

            return base.TryDeserializeNotNull(out value);
        }
    }
}





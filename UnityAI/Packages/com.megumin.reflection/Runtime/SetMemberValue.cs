using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Profiling;
using Unity.Profiling;

namespace Megumin
{
    /// <summary>
    /// 反射赋值时查找这个特性，如果设置了回调函数，则使用回调函数对成员赋值。
    /// <para>
    /// 回调方法不可以是private和static的，必须在子类中仍可以被调用，否则无法在子类型中通过反射找到该方法。
    /// </para>
    /// </summary>
    [AttributeUsage(AttributeTargets.All)]
    public class SetMemberByAttribute : Attribute
    {
        /// <summary>
        /// 回调函数应该返回一个bool，如果为true，则跳过默认的反射赋值过程。
        /// </summary>
        public string FuncName { get; set; }
        public SetMemberByAttribute(string funcName)
        {
            FuncName = funcName;
        }
    }

    /// <summary>
    /// <see cref="NonSerializedAttribute"/>会导致无法在Inspector上显示，增加一个自己的接口。
    /// 使用Megumin序列化时，忽略含有这个特性的成员。不会影响Unity默认序列化。
    /// </summary>
    [AttributeUsage(AttributeTargets.All, Inherited = true)]
    public class NonSerializedByMeguminAttribute : Attribute
    {
        public string FormatterName { get; set; }
        public NonSerializedByMeguminAttribute(string formatterName = null)
        {
            FormatterName = formatterName;
        }
    }
}

namespace Megumin.Reflection
{
    /// <summary>
    /// 序列化哪些成员委托
    /// </summary>
    /// <param backingFieldName="value"></param>
    /// <returns></returns>
    public delegate IEnumerable<InstanceMemberInfo>
        GetSerializeMembers<in T>(T value);

    public struct InstanceMemberInfo
    {
        public string Name { get; internal set; }
        public object Value { get; internal set; }
        public Type CodeType { get; internal set; }
        public MemberInfo Member { get; internal set; }
        public object Instance { get; internal set; }
        public bool IsStatic { get; internal set; }
        public bool IsGetPublic { get; internal set; }
        public bool IsSetPublic { get; internal set; }
        /// <summary>
        /// 是不是声明时的默认值
        /// </summary>
        public bool IsDefault { get; internal set; }
        public ObsoleteAttribute ObsoleteAttribute { get; internal set; }

        public void Deconstruct(out string memberName, out object memberValue, out Type memberType)
        {
            memberName = Name;
            memberValue = Value;
            memberType = CodeType;
        }

        public void Deconstruct(out string memberName, out object memberValue, out Type memberType,
            out bool isGetPublic,
            out bool isSetPublic)
        {
            memberName = Name;
            memberValue = Value;
            memberType = CodeType;
            isGetPublic = IsGetPublic;
            isSetPublic = IsSetPublic;
        }
    }

    public static class Extension_9E4697883E4048E9B612E58CDAB01B77
    {
        public const BindingFlags SetMemberFlag = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
        public const BindingFlags SetMemberCallbackFlag = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;

        /// <summary>
        /// 使用反射对实例的一个成员赋值
        /// </summary>
        /// <typeparam backingFieldName="T"></typeparam>
        /// <param backingFieldName="instance"></param>
        /// <param backingFieldName="memberName"></param>
        /// <param backingFieldName="value"></param>
        /// <returns></returns>
        public static bool TrySetMemberValue<T>(this T instance, string memberName, object value)
        {
            var instanceType = instance?.GetType();

            var m = new ProfilerMarker($"{instanceType.Name}.{memberName}");
            using var autoM = m.Auto();
 
            //类型设置了Callback 函数

            Profiler.BeginSample($"{instanceType.Name}.{memberName}--1");
            var instanceTypeMemberCallbacAttribute = instanceType?.GetCustomAttribute<SetMemberByAttribute>();
            Profiler.EndSample();

            if (string.IsNullOrEmpty(instanceTypeMemberCallbacAttribute?.FuncName) == false)
            {
                var methond = instanceType.GetMethod(instanceTypeMemberCallbacAttribute.FuncName, SetMemberCallbackFlag);
                if (methond != null)
                {
                    //此时value如果是复杂类型，可能只创建好了实例，还没有反序列化成员值
                    var success = methond.Invoke(instance, new object[] { memberName, value });
                    if (success is bool s && s == true)
                    {
                        //返回true时不再使用反射设置成员。
                        return true;
                    }
                }
                else
                {
                    Debug.LogWarning($"{instanceTypeMemberCallbacAttribute.FuncName} can not found.");
                }
            }

            //通过反射对成员赋值
            Profiler.BeginSample($"{instanceType.Name}.{memberName}--2");
            var members = instanceType?.GetMembers(SetMemberFlag);
            Profiler.EndSample();

            MemberInfo member = null;
            if (members != null)
            {
                //此处FirstOrDefault开销很大，优化为foreach。
                foreach (var elem in members)
                {
                    if (elem is MethodInfo)
                    {
                        continue;
                    }

                    if (elem.Name == memberName)
                    {
                        member = elem;
                        break;
                    }
                }
            }
            else
            {
                return false;
            }

            if (member == null)
            {
                //支持序列化成员改名
                foreach (var elem in members)
                {
                    if (elem is MethodInfo)
                    {
                        continue;
                    }

                    var attri = elem.GetCustomAttribute<UnityEngine.Serialization.FormerlySerializedAsAttribute>();
                    if (string.Equals(attri?.oldName, memberName))
                    {
                        member = elem;
                        break;
                    }

                    var attri2 = elem.GetCustomAttributes<SerializationAliasAttribute>();
                    if (attri2 != null)
                    {
                        foreach (var attri3 in attri2)
                        {
                            if (string.Equals(attri3.Alias, memberName))
                            {
                                member = elem;
                                break;
                            }
                        }
                    }
                }
            }


            if (member == null)
            {
                //没有找到成员直接返回false
#if UNITY_EDITOR || DEVELOPMENT_BUILD
                Debug.LogWarning($"{instanceType.Name}.{memberName} can not found.");
#endif
                return false;
            }

            try
            {
                Type valueType = value?.GetType();

                var perMemberCallbacAttribute = member?.GetCustomAttribute<SetMemberByAttribute>();
                if (string.IsNullOrEmpty(perMemberCallbacAttribute?.FuncName) == false)
                {
                    //针对某个成员，设置了callback
                    var methond = instanceType.GetMethod(perMemberCallbacAttribute.FuncName, SetMemberCallbackFlag);
                    if (methond != null)
                    {
                        //此时value如果是复杂类型，可能只创建好了实例，还没有反序列化成员值

                        var success = methond.Invoke(instance, new object[] { value });
                        if (success is bool s && s == true)
                        {
                            //返回true时不再使用反射设置成员。
                            return true;
                        }
                    }
                    else
                    {
                        Debug.LogWarning($"{instanceTypeMemberCallbacAttribute.FuncName} can not found.");
                    }
                }

                Profiler.BeginSample($"{instanceType.Name}.{memberName}--3");

                if (member is FieldInfo fieldInfo)
                {
                    if (value is UnityEngine.Object uobj && !uobj && fieldInfo.FieldType != valueType)
                    {
                        //空的UnityEngine.Object 反序列化后丢失真实类型。
                        //需要改为null，否则SetValue时会导致类型不匹配异常
                        value = null;
                    }

                    if (fieldInfo.FieldType.IsAssignableFrom(valueType) == false)
                    {
                        //参数类型不普配
                        Debug.LogWarning($"{member.Name}:{fieldInfo.FieldType.FullName} 参数类型不普配 value:{valueType?.FullName}");
                    }

                    fieldInfo.SetValue(instance, value);
                }
                else if (member is PropertyInfo propertyInfo)
                {
                    if (value is UnityEngine.Object uobj && !uobj && propertyInfo.PropertyType != valueType)
                    {
                        //空的UnityEngine.Object 反序列化后丢失真实类型。
                        //需要改为null，否则SetValue时会导致类型不匹配异常
                        value = null;
                    }

                    if (propertyInfo.PropertyType.IsAssignableFrom(valueType) == false)
                    {
                        //参数类型不普配
                        Debug.LogWarning($"{member.Name}:{propertyInfo.PropertyType.FullName} 参数类型不普配 value:{valueType?.FullName}");
                    }
                    propertyInfo.SetValue(instance, value);
                }

                Profiler.EndSample();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return false;
            }

            return true;
        }

        /// <summary>
        /// 默认取得需要序列化成员
        /// </summary>
        /// <typeparam backingFieldName="T"></typeparam>
        /// <param backingFieldName="instance"></param>
        /// <param name="ignoreDefaultValue">忽略值为初始化值的成员。
        /// 通常意味着实例成员值没有修改过，反序列化时不需要处理，所以不需要序列化</param>
        /// <returns></returns>
        public static IEnumerable<InstanceMemberInfo>
            GetSerializeMembers<T>(this T instance,
                                   bool ignoreDefaultValue = true,
                                   bool fullInfo = false)
        {
            if (instance == null)
            {
                yield break;
            }

            var instanceType = instance.GetType();

            if (instanceType.IsPrimitive
                || instanceType == typeof(string))
            {
                //基元类型不要反射查找成员。
                yield break;
            }

            if (instance is IDictionary dictionary)
            {
                Debug.LogError($"不支持字典");
                yield break;
            }
            else if (instance is IList list)
            {
                Type memberType = null;
                if (instanceType.IsArray)
                {
                    memberType = instanceType.GetElementType();
                }
                else
                {
                    memberType = instanceType.GetGenericArguments()?[0];
                }

                if (memberType == null)
                {
                    Debug.LogError($"找不到特化类型");
                    yield break;
                }

                for (int i = 0; i < list.Count; i++)
                {
                    object item = list[i];
                    var memberInstanceType = memberType;
                    if (item != null)
                    {
                        //集合中可能是多态对象。
                        memberInstanceType = item.GetType();
                    }

                    InstanceMemberInfo info = new();
                    info.Name = i.ToString();
                    info.Value = item;
                    info.CodeType = memberInstanceType;
                    info.Member = null;
                    info.Instance = instance;
                    info.IsStatic = false;
                    info.IsGetPublic = true;
                    info.IsSetPublic = true;
                    yield return info;
                }
            }
            else
            {
                var p = from m in instanceType.GetMembers(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                        where m is FieldInfo || m is PropertyInfo
                        orderby m.MetadataToken
                        select m;
                var members = p.ToList();

                ///用于忽略默认值参数
                object defualtValueInstance = null;
                try
                {
                    defualtValueInstance = Activator.CreateInstance(instanceType);
                }
                catch (Exception)
                {
                    Debug.LogError($"{instanceType} cannot Activator.CreateInstance!");
                    throw;
                }

                foreach (var member in members)
                {
                    object memberValue = null;
                    object defaultMemberValue = null;
                    Type memberCodeType = null;
                    var isGetPublic = false;
                    var isSetPublic = false;

                    if (member is FieldInfo field)
                    {
                        if (field.CanSerializable() == false)
                        {
                            continue;
                        }

                        memberCodeType = field.FieldType;
                        memberValue = field.GetValue(instance);
                        defaultMemberValue = field.GetValue(defualtValueInstance);
                        isGetPublic = field.IsPublic;
                        isSetPublic = field.IsPublic;
                    }
                    else if (member is PropertyInfo property)
                    {
                        //https://stackoverflow.com/questions/8817070/is-it-possible-to-access-backing-fields-behind-auto-implemented-properties
                        string backingFieldName = $"<{property.Name}>k__BackingField";
                        //这里一定要用property.DeclaringTyp，否则继承的类型无法获取后备字段
                        var backingField = property.DeclaringType.GetField(backingFieldName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                        if (backingField == null)
                        {
                            continue;
                        }

                        if (members.Contains(backingField))
                        {
                            //能正常获取后背字段的时候，不序列化属性，以后背字段为准
                            continue;
                        }

                        if (backingField.CanSerializable() == false)
                        {
                            continue;
                        }

                        memberCodeType = property.PropertyType;
                        memberValue = property.GetValue(instance);
                        defaultMemberValue = property.GetValue(defualtValueInstance);
                        isGetPublic = property.CanRead ? property.GetMethod.IsPublic : false;
                        isSetPublic = property.CanWrite ? property.SetMethod.IsPublic : false;
                    }

                    if (memberValue is IDictionary)
                    {
                        //暂时不支持字典
                        continue;
                    }

                    InstanceMemberInfo info = new();

                    //注意：这里不能因为memberValue == null,就跳过序列化。
                    //一个可能的用例是，字段声明是默认不是null，后期用户赋值为null。
                    //如果跳过序列化会导致反射构建实例是null的字段初始化为默认值。
                    if (memberValue == defaultMemberValue
                        || (memberValue?.Equals(defaultMemberValue) ?? false))
                    {
                        info.IsDefault = true;

                        if (ignoreDefaultValue)
                        {
                            //Debug.Log($"值为初始值或者默认值没必要保存");
                            continue;
                        }
                    }

                    info.Name = member.Name;
                    info.Value = memberValue;
                    info.CodeType = memberCodeType;
                    info.Member = member;
                    info.IsStatic = member.IsStaticMember();
                    info.IsGetPublic = isGetPublic;
                    info.IsSetPublic = isSetPublic;

                    info.ObsoleteAttribute = member.GetCustomAttribute<ObsoleteAttribute>();

                    if (fullInfo == false && instanceType.IsValueType)
                    {
                        //减少装箱。
                    }
                    else
                    {
                        info.Instance = instance;
                    }

                    yield return info;
                }
            }
        }

        public static bool CanSerializable(this FieldInfo field)
        {
            var hasSerializeField = field.IsDefined(typeof(SerializeField), true);
            if (hasSerializeField)
            {
                return true;
            }

            var hasSerializeReference = field.IsDefined(typeof(SerializeReference), true);
            if (hasSerializeReference)
            {
                return true;
            }

            var hasNonSerialized = field.IsDefined(typeof(NonSerializedAttribute), true);
            if (hasNonSerialized)
            {
                return false;
            }

            var hasNonSerializedByMegumin = field.IsDefined(typeof(NonSerializedByMeguminAttribute), true);
            if (hasNonSerializedByMegumin)
            {
                return false;
            }

            return field.IsPublic;
        }


        public static bool IsStaticMember(this MemberInfo member)
        {
            if (member is MethodInfo method)
            {
                return method.IsStatic;
            }
            else if (member is FieldInfo field)
            {
                return field.IsStatic;
            }
            else if (member is PropertyInfo property)
            {
                if (property.CanRead)
                {
                    return property.GetMethod.IsStatic;
                }

                if (property.CanWrite)
                {
                    return property.SetMethod.IsStatic;
                }
            }

            return false;
        }

        public static bool IsGetPublic(this MemberInfo member)
        {
            if (member is MethodInfo method)
            {
                return method.IsPublic;
            }
            else if (member is FieldInfo field)
            {
                return field.IsPublic;
            }
            else if (member is PropertyInfo property)
            {
                if (property.CanRead)
                {
                    return property.GetMethod.IsPublic;
                }
            }

            return false;
        }

        public static bool IsSetPublic(this MemberInfo member)
        {
            if (member is MethodInfo method)
            {
                return method.IsPublic;
            }
            else if (member is FieldInfo field)
            {
                return field.IsPublic;
            }
            else if (member is PropertyInfo property)
            {
                if (property.CanWrite)
                {
                    return property.SetMethod.IsPublic;
                }
            }

            return false;
        }

        public static Type GetMemberType(this MemberInfo member)
        {
            if (member is MethodInfo method)
            {
                return method.ReturnType;
            }
            else if (member is FieldInfo field)
            {
                return field.FieldType;
            }
            else if (member is PropertyInfo property)
            {
                return property.PropertyType;
            }

            return null;
        }

        /// <summary>
        /// 预热反射调用
        /// </summary>
        /// <param name="type"></param>
        public static void WarmUpReflection_TrySetMember(this Type type)
        {
            if (type == null)
            {
                return;
            }

            var name = type.Name;
            var fullName = type.FullName;

            type.GetCustomAttribute<SetMemberByAttribute>(); //0.1ms+
            //预热反射成员
            var members = type.GetMembers(SetMemberCallbackFlag);

            object[] attris = null;
            foreach (var item in members)
            {
                attris = item.GetCustomAttributes(true);
                var a1 = item.GetCustomAttribute<UnityEngine.Serialization.FormerlySerializedAsAttribute>();//0.1ms+
                var a2 = item.GetCustomAttribute<SerializationAliasAttribute>();//0.1ms+
                var a3 = item.GetCustomAttribute<SetMemberByAttribute>();//0.1ms+
            }

            var fields = type.GetFields(SetMemberCallbackFlag);
            foreach (var item in fields)
            {
                attris = item.GetCustomAttributes(true);
            }

            var props = type.GetProperties(SetMemberCallbackFlag);
            foreach (var item in props)
            {
                attris = item.GetCustomAttributes(true);
            }

            var methods = type.GetMethods(SetMemberCallbackFlag);
            foreach (var item in methods)
            {
                attris = item.GetCustomAttributes(true);
            }
        }
    }
}





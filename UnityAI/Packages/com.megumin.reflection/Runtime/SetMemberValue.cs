using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Megumin.Reflection
{
    /// <summary>
    /// 反射赋值是会查找这个特性，如果设置了回调函数，则使用回调函数对成员赋值。
    /// <para>
    /// 回调方法不可以是private和static的，必须在子类中仍可以被调用，否则无法在子类型中通过反射找到该方法。
    /// </para>
    /// </summary>
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
    /// 序列化哪些成员委托
    /// </summary>
    /// <param backingFieldName="value"></param>
    /// <returns></returns>
    public delegate IEnumerable<(string MemberName, object MemberValue, Type MemberType)>
        GetSerializeMembers<in T>(T value);

    public static class Extension_9E4697883E4048E9B612E58CDAB01B77
    {
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
            const BindingFlags BindingAttr = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
            const BindingFlags callbackflag = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
            var instanceType = instance?.GetType();

            //类型设置了Callback 函数
            var instanceTypeMemberCallbacAttribute = instanceType?.GetCustomAttribute<SetMemberByAttribute>();
            if (string.IsNullOrEmpty(instanceTypeMemberCallbacAttribute?.FuncName) == false)
            {
                var methond = instanceType.GetMethod(instanceTypeMemberCallbacAttribute.FuncName, callbackflag);
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
            var members = instanceType?.GetMembers(BindingAttr);
            var member = members?.FirstOrDefault(elem => elem.Name == memberName);

            if (member == null)
            {
                //支持序列化成员改名
                foreach (var elem in members)
                {
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

            try
            {
                Type valueType = value?.GetType();

                var perMemberCallbacAttribute = member?.GetCustomAttribute<SetMemberByAttribute>();
                if (string.IsNullOrEmpty(perMemberCallbacAttribute?.FuncName) == false)
                {
                    //针对某个成员，设置了callback
                    var methond = instanceType.GetMethod(perMemberCallbacAttribute.FuncName, callbackflag);
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
        /// <returns></returns>
        public static IEnumerable<(string MemberName, object MemberValue, Type MemberType)>
            GetSerializeMembers<T>(this T instance)
        {
            if (instance == null)
            {
                yield break;
            }

            var instanceType = instance.GetType();
            var p = from m in instanceType.GetMembers(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                    where m is FieldInfo || m is PropertyInfo
                    orderby m.MetadataToken
                    select m;
            var members = p.ToList();

            ///用于忽略默认值参数
            var defualtValueInstance = Activator.CreateInstance(instanceType);

            foreach (var member in members)
            {
                object memberValue = null;
                object defaultMemberValue = null;
                Type memberCodeType = null;

                if (member is FieldInfo field)
                {
                    if (field.CanSerializable() == false)
                    {
                        continue;
                    }

                    memberCodeType = field.FieldType;
                    memberValue = field.GetValue(instance);
                    defaultMemberValue = field.GetValue(defualtValueInstance);
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
                }

                //注意：这里不能因为memberValue == null,就跳过序列化。
                //一个可能的用例是，字段声明是默认不是null，后期用户赋值为null。
                //如果跳过序列化会导致反射构建实例是null的字段初始化为默认值。
                if (memberValue == defaultMemberValue
                    || (memberValue?.Equals(defaultMemberValue) ?? false))
                {
                    //Debug.Log($"值为初始值或者默认值没必要保存");
                    continue;
                }

                if (memberValue is IDictionary)
                {
                    //暂时不支持字典
                    continue;
                }

                yield return (member.Name, memberValue, memberCodeType);
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
    }
}





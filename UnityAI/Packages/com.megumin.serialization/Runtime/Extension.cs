using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Megumin.Serialization
{
    internal static class Extension_9E4697883E4048E9B612E58CDAB01B77
    {
        /// <summary>
        /// 使用反射对实例的一个成员赋值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance"></param>
        /// <param name="memberName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool TrySetMemberValue<T>(this T instance, string memberName, object value)
        {
            var member = instance?.GetType().GetMember(memberName)?.FirstOrDefault();
            try
            {
                if (member is FieldInfo fieldInfo)
                {
                    if (value != null && fieldInfo.FieldType.IsAssignableFrom(value.GetType()))
                    {
                        //参数类型不普配
                        Debug.LogError("参数类型不普配");
                    }
                    fieldInfo.SetValue(instance, value);
                }
                else if (member is PropertyInfo propertyInfo)
                {
                    if (value != null && propertyInfo.PropertyType.IsAssignableFrom(value.GetType()))
                    {
                        //参数类型不普配
                        Debug.LogError("参数类型不普配");
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
    }
}

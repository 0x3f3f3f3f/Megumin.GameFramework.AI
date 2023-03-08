using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Megumin.GameFramework.AI.BehaviorTree;
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
        public static IParameterData Serialize(MemberInfo member, object instance, object defualtValueInstance)
        {
            Debug.Log(member);

            if (member is FieldInfo field)
            {
                var value = field.GetValue(instance);
                if (value.Equals(field.GetValue(defualtValueInstance)))
                {
                    Debug.Log($"值为初始值或者默认值没必要保存");
                }
                else
                {
                    var valueActualType = value.GetType();
                    if (valueActualType == typeof(int))
                    {
                        IntPara data = new();
                        data.MemberName = member.Name;
                        data.Value = (int)value;
                        return data;
                    }
                    else if (valueActualType == typeof(UnityEngine.Object))
                    {
                        UnityObjectParameterData data = new();
                        data.MemberName = member.Name;
                        data.Value = (UnityEngine.Object)value;
                        return data;
                    }
                    else
                    {
                        CustomParameterData data = new();
                        data.MemberName = member.Name;
                        data.TypeName = field.FieldType.FullName;
                        if (value != null)
                        {
                            //这里一定要取值得真实类型，解决多态序列化
                            if (Formater.TryGet(value.GetType(), out IFormater2String iformater))
                            {
                                data.TypeName = value.GetType().FullName;
                                data.Value = iformater.Serialize(value);
                            }
                            else
                            {
                                Debug.LogError($"{member.Name} 没找到Iformater");
                            }
                        }
                        else
                        {
                            data.IsNull = true;
                            //引用类型并且值为null
                        }
                        return data;
                    }
                }

            }

            return null;
        }

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
    public class IntPara : GenericParameterData<int> { }

    public class UnityObjectParameterData : GenericParameterData<UnityEngine.Object> { }

    [Serializable]
    public class CustomParameterData : ParameterData
    {
        [Flags]
        public enum Mark
        {
            Nono = 0,
            IsNull = 1 << 0,
            UnityObject = 1 << 1,
        }

        public string TypeName;
        public string Value;
        public UnityEngine.Object refrenceObject;
        public bool IsNull;

        public bool TryDeserialize(out object value)
        {
            if (Formater.TryGet(TypeName, out var iformater))
            {
                return iformater.TryDeserialize(Value, out value);
            }
            value = default;
            return false;
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


}

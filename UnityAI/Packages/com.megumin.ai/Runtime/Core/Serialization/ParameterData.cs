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
    [Serializable]
    public class ParameterData
    {
        [Flags]
        public enum Mark
        {
            Nono = 0,
            IsNull = 1 << 0,
            UnityObject = 1 << 1,
        }
        public string ParamName;
        public string TypeName;
        public string Value;
        public UnityEngine.Object refrenceObject;
        public bool IsNull;

        public static ParameterData Serialize(MemberInfo member, object node, object defualtValueNode)
        {
            Debug.Log(member);

            if (member is FieldInfo field)
            {
                var value = field.GetValue(node);
                if (value.Equals(field.GetValue(defualtValueNode)))
                {
                    Debug.Log($"值为初始值或者默认值没必要保存");
                }
                else
                {
                    ParameterData data = new();
                    data.ParamName = member.Name;
                    data.TypeName = field.FieldType.FullName;
                    if (value != null)
                    {
                        //这里一定要取值得真实类型，解决多态序列化
                        if (Formater.TryGet(value.GetType(), out Iformater iformater))
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

            return null;
        }

        public bool TreDes(out object value)
        {
            if (Formater.TryGet(TypeName,out var iformater))
            {
                return iformater.TreDes(Value,out value);
            }
            value = default;
            return false;
        }

        public bool Instantiate(object instance)
        {
            if (instance == null)
            {
                return false;
            }

            //Todo: 要不要使用TokenID查找
            var member = instance.GetType().GetMember(ParamName).FirstOrDefault();
            if (member != null && TreDes(out var value))
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

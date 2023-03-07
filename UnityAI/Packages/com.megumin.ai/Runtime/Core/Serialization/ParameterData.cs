using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
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
                if (field.FieldType.IsClass)
                {
                    var value = field.GetValue(node);
                    if (value == field.GetValue(defualtValueNode))
                    {
                        Debug.Log($"值为初始值或者默认值没必要保存");
                    }
                    else
                    {
                        ParameterData paramAsset = new();
                        paramAsset.ParamName = member.Name;
                        paramAsset.TypeName = field.FieldType.FullName;
                        if (value != null)
                        {
                            if (Formater.TryGet(value.GetType(), out Iformater iformater))
                            {
                                paramAsset.TypeName = value.GetType().FullName;
                                paramAsset.Value = iformater.Serialize(value);
                            }
                            else
                            {
                                Debug.LogError($"{member.Name} 没找到Iformater");
                            }
                        }
                        else
                        {
                            paramAsset.IsNull = true;
                            //引用类型并且值为null
                        }
                        return paramAsset;
                    }


                }

            }

            return null;
        }
    }
}

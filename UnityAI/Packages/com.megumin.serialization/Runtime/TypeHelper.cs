using System;
using System.Collections;
using System.Collections.Generic;
using Megumin.Reflection;
using UnityEngine;

namespace Megumin.Serialization
{
    public class TypeHelper
    {
        /// <summary>
        /// 用于重命名时反序列化。用于解决命名空间改变后反序列化已有文件问题。
        /// <para/> 当类全名包含Key时，使用Value替换类全名中的Key
        /// </summary>
        public static List<(string Key, string Value)> ReplacePartialName = new()
        {
            ( "Megumin.GameFramework.AI" , "Megumin.AI" ),
        };

        /// <summary>
        /// 找不到类名时尝试更换命名空间。
        /// <para/> 当旧的命名空间普配时，使用新的命名空间替换旧的命名空间。
        /// </summary>
        public static List<(string OldNamespace, string ReplaceNamespace)> ReplaceNamespace = new()
        {
            ( "" , "Megumin.AI.BehaviorTree." ),
            ( "" , "Megumin.AI." ),
        };

        public static bool TryGetType(string typeFullName, out Type type)
        {
            if (TypeCache.TryGetType(typeFullName, out type))
            {
                return true;
            }
            else
            {
                //没有找到类型时，尝试遍历fallback字典，映射到重命名之前的名字。用于解决命名空间改变。
                foreach (var item in ReplacePartialName)
                {
                    if (typeFullName.Contains(item.Key))
                    {
                        var newTypeFullName = typeFullName.Replace(item.Key, item.Value);
                        if (TypeCache.TryGetType(newTypeFullName, out type))
                        {
                            return true;
                        }
                    }
                }

                if (ReplaceNamespace.Count > 0)
                {
                    //找不到类名时尝试更换命名空间
                    var (Namespace, TypeName) = TypeCache.SplitNamespace(typeFullName);
                    foreach (var item in ReplaceNamespace)
                    {
                        if (item.OldNamespace == Namespace)
                        {
                            var newTypeFullName = item.ReplaceNamespace + TypeName;
                            if (TypeCache.TryGetType(newTypeFullName, out type))
                            {
                                return true;
                            }
                        }
                    }
                }
            }

            type = default;
            return false;
        }

        public static Type GetType(string typeFullName)
        {
            TryGetType(typeFullName, out var type);
            return type;
        }
    }
}

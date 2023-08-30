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
        /// </summary>
        public static Dictionary<string, string> FallbackRename = new()
        {
            { "Megumin.GameFramework.AI", "Megumin.AI" }
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
                foreach (var item in FallbackRename)
                {
                    if (typeFullName.Contains(item.Key))
                    {
                        var fallbackTypeString = typeFullName.Replace(item.Key, item.Value);
                        if (TypeCache.TryGetType(fallbackTypeString, out type))
                        {
                            return true;
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

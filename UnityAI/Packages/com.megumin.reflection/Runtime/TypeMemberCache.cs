using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Megumin.Reflection
{
    /// <summary>
    /// Copy From Megumin.Cache
    /// </summary>
    /// <typeparam name="K"></typeparam>
    /// <typeparam name="V"></typeparam>
    public abstract class MemberCache<K, V>
    {
        public virtual bool Load()
        {
            return true;
        }

        public V Get(K key, bool forceReCache = false, object option = null)
        {
            if (forceReCache == false && TryGetCache(in key, out var result, option))
            {
                return result;
            }
            else
            {
                return ReCache(key, forceReCache, option);
            }
        }

        /// <summary>
        /// 重新计算结果，并更新到缓存中
        /// </summary>
        /// <param name="key"></param>
        /// <param name="forceReCache"></param>
        /// <param name="option"></param>
        /// <returns></returns>
        public virtual V ReCache(K key, bool forceReCache = false, object option = null)
        {
            var result = Calculate(key, forceReCache, option);
            UpdateCache(in key, result, forceReCache, option);
            return result;
        }

        /// <summary>
        /// 计算结果
        /// </summary>
        /// <param name="key"></param>
        /// <param name="forceReCache"></param>
        /// <param name="option"></param>
        /// <returns></returns>
        public abstract V Calculate(K key, bool forceReCache = false, object option = null);

        protected static Dictionary<K, V> CacheDic { get; } = new();

        public bool TryGetCache(in K key, out V value, object option = null)
        {
            return CacheDic.TryGetValue(key, out value);
        }

        public void UpdateCache(in K key, V value, bool forceReCache = false, object option = null)
        {
            CacheDic[key] = value;
        }

        public bool Clear()
        {
            CacheDic.Clear();
            return true;
        }
    }

    public class TypeAllMembersCache : MemberCache<(Type, BindingFlags), MemberInfo[]>
    {
        public override MemberInfo[] Calculate((Type, BindingFlags) key, bool forceReCache = false, object option = null)
        {
            return key.Item1.GetMembers(key.Item2);
        }
    }

    public class MemberAttributeCache : MemberCache<(MemberInfo, Type), Attribute>
    {
        public override Attribute Calculate((MemberInfo, Type) key, bool forceReCache = false, object option = null)
        {
            return key.Item1.GetCustomAttribute(key.Item2);
        }
    }

    public static class TypeMemberCache
    {
        public static readonly TypeAllMembersCache AllMember = new TypeAllMembersCache();
        public static readonly MemberAttributeCache AttributeCache = new MemberAttributeCache();

        /// <summary>
        /// 通过缓存反射获取所有成员
        /// </summary>
        /// <param name="type"></param>
        /// <param name="flags"></param>
        /// <param name="useCache"></param>
        /// <returns></returns>
        public static MemberInfo[] GetCacheMembers(this Type type, BindingFlags flags, bool useCache = true)
        {
            if (type == null)
            {
                return null;
            }

            if (useCache)
            {
                return AllMember.Get((type, flags));
            }
            else
            {
                return type.GetMembers(flags);
            }
        }

        public static T GetCacheCustomAttribute<T>(this MemberInfo memberInfo, bool useCache = true)
            where T : Attribute
        {
            if (memberInfo == null)
            {
                return null;
            }

            if (useCache)
            {
                return AttributeCache.Get((memberInfo, typeof(T))) as T;
            }
            else
            {
                return memberInfo.GetCustomAttribute<T>();
            }
        }

        //GetCustomAttributes 调用很少，不值得做缓存。
    }
}

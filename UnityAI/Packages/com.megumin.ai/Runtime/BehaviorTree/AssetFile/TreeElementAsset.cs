using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Megumin.GameFramework.AI.Serialization;
using Megumin.Serialization;
using UnityEngine;

namespace Megumin.GameFramework.AI.BehaviorTree
{
    public class TreeElementAsset
    {
        public List<string> StringCallbackMemberData = new();

        public static void BeforeSerializeMember<T>(object instance,
                                          List<string> callbackIgnoreMember,
                                          List<T> callbackMemberData)
        {
            if (instance is ISerializationCallbackReceiver<T> callbackReceiver)
            {
                callbackReceiver.OnBeforeSerialize(callbackMemberData, callbackIgnoreMember);
            }
        }

        public static void AfterDeserializeMember<T>(object instance, List<T> callbackMemberData)
        {
            if (instance is ISerializationCallbackReceiver<T> callbackReceiver)
            {
                callbackReceiver.OnAfterDeserialize(callbackMemberData);
            }
        }

        public void SerializeMember(object instance,
                                    List<string> ignoreMember,
                                    List<CollectionSerializationData> memberData,
                                    List<CollectionSerializationData> callbackMemberData)
        {
            //保存参数
            //https://github.com/dotnet/runtime/issues/46272

            List<string> callbackIgnoreMember = new();

            if (instance is ISerializationCallbackReceiver serializationCallbackReceiver)
            {
                serializationCallbackReceiver.OnBeforeSerialize();
            }

            BeforeSerializeMember(instance, callbackIgnoreMember, callbackMemberData);
            BeforeSerializeMember(instance, callbackIgnoreMember, StringCallbackMemberData);

            var nodeType = instance.GetType();
            var p = from m in nodeType.GetMembers()
                    where m is FieldInfo || m is PropertyInfo
                    orderby m.MetadataToken
                    select m;
            var members = p.ToList();

            ///用于忽略默认值参数
            var defualtValueInstance = Activator.CreateInstance(nodeType);

            foreach (var member in members)
            {
                if (ignoreMember?.Contains(member.Name) ?? false)
                {
                    //Debug.LogError($"忽略的参数 {member.Name}");
                    continue;
                }

                if (callbackIgnoreMember.Contains(member.Name))
                {
                    //Debug.LogError($"忽略的参数 {member.Name}");
                    continue;
                }

                ////如果是Data<>
                //object memberValue = null;
                //object defaultMemberValue = null;
                //Type memberType = null;

                //if (member is FieldInfo field)
                //{
                //    memberType = field.FieldType;
                //    memberValue = field.GetValue(instance);
                //    defaultMemberValue = field.GetValue(defualtValueInstance);
                //}
                //else if (member is PropertyInfo property)
                //{
                //    memberType = property.PropertyType;
                //    memberValue = property.GetValue(instance);
                //    defaultMemberValue = property.GetValue(defualtValueInstance);
                //}

                //if (memberValue == defaultMemberValue
                //    || (memberValue?.Equals(defaultMemberValue) ?? false))
                //{
                //    //Debug.Log($"值为初始值或者默认值没必要保存");
                //}

                //if (typeof(MMData<>).IsAssignableFrom(memberType)
                //    || typeof(MMData<>).IsAssignableFrom(memberValue?.GetType()))
                //{
                //    //如果是Data<>
                //    //特殊序列化
                //    CollectionSerializationData valueData = new();
                //    if (valueData.TrySerializeMemberValue(memberValue))
                //    {

                //    }

                //    MMDataSerializationData mmdata = new();
                //    if (mmdata.TrySerializeMemberValue(memberValue))
                //    {
                //        VariableTable.Add(data);
                //    }
                //}

                if (TrySerializeMemberValue(member, instance, defualtValueInstance, out var data))
                {
                    memberData.Add(data);
                }
            }
        }

        /// <summary>
        /// https://blog.unity.com/technology/serialization-in-unity
        /// </summary>
        /// <param name="member"></param>
        /// <param name="instance"></param>
        /// <param name="defualtValueInstance"></param>
        /// <returns></returns>
        bool TrySerializeMemberValue(MemberInfo member,
            object instance,
            object defualtValueInstance,
            out CollectionSerializationData data)
        {
            //Debug.Log(member);

            data = new();
            object memberValue = null;
            object defaultMemberValue = null;

            if (member is FieldInfo field)
            {
                memberValue = field.GetValue(instance);
                defaultMemberValue = field.GetValue(defualtValueInstance);
            }
            else if (member is PropertyInfo property)
            {
                memberValue = property.GetValue(instance);
                defaultMemberValue = property.GetValue(defualtValueInstance);
            }

            if (memberValue == defaultMemberValue
                || (memberValue?.Equals(defaultMemberValue) ?? false))
            {
                //Debug.Log($"值为初始值或者默认值没必要保存");
            }
            else
            {
                return data.TrySerialize(member.Name, memberValue);
            }

            return false;
        }

        public void DeserializeMember(object instance,
                                      List<CollectionSerializationData> memberData,
                                      List<CollectionSerializationData> callbackMemberData)
        {
            if (instance == null)
            {
                return;
            }
            //反序列化参数
            foreach (var param in memberData)
            {
                if (param == null)
                {
                    continue;
                }

                //Todo: 要不要使用TokenID查找
                var member = instance.GetType().GetMember(param.Name)?.FirstOrDefault();
                if (member != null && param.TryDeserialize(out var value))
                {
                    if (member is FieldInfo fieldInfo)
                    {
                        fieldInfo.SetValue(instance, value);
                    }
                    else if (member is PropertyInfo propertyInfo)
                    {
                        propertyInfo.SetValue(instance, value);
                    }
                }
            }

            AfterDeserializeMember(instance, StringCallbackMemberData);
            AfterDeserializeMember(instance, callbackMemberData);

            if (instance is ISerializationCallbackReceiver serializationCallbackReceiver)
            {
                serializationCallbackReceiver.OnAfterDeserialize();
            }
        }
    }
}

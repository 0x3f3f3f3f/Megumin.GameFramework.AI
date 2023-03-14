using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Megumin.Serialization;
using UnityEngine;
using Megumin;

namespace Megumin.GameFramework.AI.BehaviorTree
{
    public class TreeElementAsset
    {
        public List<string> StringCallbackMemberData = new();
        public List<MMDataSerializationData> MMdata = new();

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
            MMdata.Clear();
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

                //这段代码不能抽象到SerializationData中，下面会涉及到很多业务特殊类，需要特定序列化
                object memberValue = null;
                object defaultMemberValue = null;
                Type memberCodeType = null;

                if (member is FieldInfo field)
                {
                    memberCodeType = field.FieldType;
                    memberValue = field.GetValue(instance);
                    defaultMemberValue = field.GetValue(defualtValueInstance);
                }
                else if (member is PropertyInfo property)
                {
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

                //如果是IMMDataable
                //特殊序列化
                if (memberValue is IMMDataable variable)
                {
                    var data = variable.GetValue();
                    CollectionSerializationData valueData = new();
                    if (valueData.TrySerialize("Value", data))
                    {
                        MMDataSerializationData mmdata = new();
                        mmdata.Data = valueData;
                        mmdata.MemberName = member.Name;
                        mmdata.TypeName = variable.GetType().FullName;
                        if (memberValue is IBindable bindable)
                        {
                            mmdata.Path = bindable.Path;
                        }

                        if (memberValue is IRefSharedable sharedable)
                        {
                            mmdata.RefName = sharedable.Name;
                        }
                        MMdata.Add(mmdata);
                    }
                }
                else
                {
                    CollectionSerializationData data = new();
                    if (data.TrySerialize(member.Name, memberValue))
                    {
                        memberData.Add(data);
                    }
                }
            }
        }

        public void DeserializeMember(object instance,
                                      List<CollectionSerializationData> memberData,
                                      List<CollectionSerializationData> callbackMemberData, 
                                      IRefFinder refFinder)
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

            //反序列化公开参数
            foreach (var variableData in MMdata)
            {
                //Todo: 要不要使用TokenID查找
                var member = instance.GetType().GetMember(variableData.MemberName)?.FirstOrDefault();
                if (member != null && variableData.TryDeserialize(out var variable, refFinder))
                {
                    if (member is FieldInfo fieldInfo)
                    {
                        fieldInfo.SetValue(instance, variable);
                    }
                    else if (member is PropertyInfo propertyInfo)
                    {
                        propertyInfo.SetValue(instance, variable);
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

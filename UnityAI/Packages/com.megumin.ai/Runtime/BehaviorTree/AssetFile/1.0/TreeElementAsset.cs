using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Megumin.Serialization;
using UnityEngine;
using Megumin;
using Megumin.Binding;

namespace Megumin.GameFramework.AI.BehaviorTree
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// 目前框架<see cref="ISerializationCallbackReceiver"/>只支持<see cref="string"/>和<see cref="CollectionSerializationData"/>类型。
    /// 当类型时string时，用户可以可以fallback到json序列化。
    /// </remarks>
    [Obsolete]
    public class TreeElementAsset
    {
        public List<string> StringCallbackMemberData = new();
        public List<RefVariableSerializationData> MMdata = new();

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
                    //Debug.LogError($"忽略的参数 {member.RefName}");
                    continue;
                }

                if (callbackIgnoreMember.Contains(member.Name))
                {
                    //Debug.LogError($"忽略的参数 {member.RefName}");
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

                //如果是参数绑定
                //特殊序列化
                if (memberValue is IVariable variable)
                {
                    RefVariableSerializationData mmdata = new();
                    if (mmdata.TrySerialize(member.Name, variable))
                    {
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

        public void DeserializeMember(BehaviorTreeElement instance,
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
                    try
                    {
                        if (member is FieldInfo fieldInfo)
                        {
                            if (variable != null && !fieldInfo.FieldType.IsAssignableFrom(variable.GetType()))
                            {
                                //参数类型不普配
                                Debug.LogError("参数类型不普配");
                            }
                            fieldInfo.SetValue(instance, variable);
                        }
                        else if (member is PropertyInfo propertyInfo)
                        {
                            propertyInfo.SetValue(instance, variable);
                        }

                        if (variable is IBindingParseable bindingParseable)
                        {
                            instance.Tree.AllBindingParseable.Add(bindingParseable);
                        }

                        if (variable is IBindAgentable bindAgentable)
                        {
                            instance.Tree.AllBindAgentable.Add(bindAgentable);
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.LogError(e);
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

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
                                      List<CollectionSerilizeData> memberData,
                                      List<CollectionSerilizeData> callbackMemberData)
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

                CollectionSerilizeData data = new();
                if (data.TrySerialize(member,instance, defualtValueInstance))
                {
                    memberData.Add(data);
                }
            }
        }

        public void DeserializeMember(object instance,
                                          List<CollectionSerilizeData> memberData,
                                          List<CollectionSerilizeData> callbackMemberData)
        {
            //反序列化参数
            foreach (var param in memberData)
            {
                param?.Instantiate(instance);
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

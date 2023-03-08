using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Megumin.GameFramework.AI.Serialization;
using UnityEngine;

namespace Megumin.GameFramework.AI.BehaviorTree
{
    public class TreeElementAsset
    {
        public static void SerializeMember(object instance,
                                          List<string> ignoreMember,
                                          List<CustomParameterData> memberData,
                                          List<CustomParameterData> callbackMemberData)
        {
            //保存参数
            //https://github.com/dotnet/runtime/issues/46272

            List<string> callbackIgnoreMember = new();

            if (instance is ISerializationCallbackReceiver serializationCallbackReceiver)
            {
                serializationCallbackReceiver.OnBeforeSerialize();
            }

            if (instance is IParameterDataSerializationCallbackReceiver callbackReceiver)
            {
                callbackReceiver.OnBeforeSerialize(callbackMemberData, callbackIgnoreMember);
            }

            var nodeType = instance.GetType();
            var p = from m in nodeType.GetMembers()
                    where m is FieldInfo || m is PropertyInfo
                    orderby m.MetadataToken
                    select m;
            var members = p.ToList();

            ///用于忽略默认值参数
            var defualtValueNode = Activator.CreateInstance(nodeType);

            foreach (var member in members)
            {
                if (ignoreMember?.Contains(member.Name) ?? false)
                {
                    Debug.LogError($"忽略的参数 {member.Name}");
                    continue;
                }

                if (callbackIgnoreMember.Contains(member.Name))
                {
                    Debug.LogError($"忽略的参数 {member.Name}");
                    continue;
                }

                var paramData = ParameterData.Serialize(member, instance, defualtValueNode);
                if (paramData != null)
                {
                    memberData.Add(paramData);
                }
            }
        }



        public static void DeserializeMember(object instance,
                                          List<CustomParameterData> memberData,
                                          List<CustomParameterData> callbackMemberData)
        {
            //反序列化参数
            foreach (var param in memberData)
            {
                param?.Instantiate(instance);
            }

            if (instance is IParameterDataSerializationCallbackReceiver callbackReceiver)
            {
                callbackReceiver.OnAfterDeserialize(callbackMemberData);
            }
        }
    }
}

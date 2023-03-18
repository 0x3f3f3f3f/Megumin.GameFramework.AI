using System;
using System.Collections;
using System.Collections.Generic;
using Megumin.Serialization;
using UnityEngine;

namespace Megumin.GameFramework.AI.BehaviorTree
{
    public class BehaviorTreeAsset_1_0_1 : ScriptableObject
    {
        public List<UnityObjRef> UnityObjectRef;
        public List<ObjData> datas;

        public void Se(BehaviorTree tree)
        {
            Dictionary<object, string> cahce = new();
            Stack<(string name, object value)> needS = new();
            List<UnityObjRef> objRefs = new();

            foreach (var variable in tree.Variable.Table)
            {
                cahce.Add(variable, variable.RefName);
                needS.Push((variable.RefName, variable));
            }

            foreach (var node in tree.AllNodes)
            {
                cahce.Add(node, node.GUID);
                needS.Push((node.GUID, node));

                foreach (var decorator in node.Decorators)
                {
                    cahce.Add(decorator, decorator.GUID);
                    needS.Push((decorator.GUID, decorator));
                }
            }

            List<ObjData> AllSedRefData = new();
            while (needS.Count > 0)
            {
                if (AllSedRefData.Count > 1000)
                {
                    Debug.LogError($"Too Large!!");
                    break;
                }

                var item = needS.Pop();
                ObjData data = new ObjData();
                if (data.TreS(item.name, item.value, needS, objRefs, cahce))
                {
                    AllSedRefData.Add(data);
                }
            }

            AllSedRefData.Sort();
            datas = AllSedRefData;
            UnityObjectRef = objRefs;
        }





        [Serializable]
        public class UnityObjRef
        {
            public string Name;
            public UnityEngine.Object Ref;
        }

        [Serializable]
        public class Basic
        {
            public string Name;
            public string Type;
            public string Value;
        }

        [Serializable]
        public class ObjData : IComparable<ObjData>
        {
            public string Name;
            public string Type;
            public List<Basic> Member;

            const string NullType = "$null";
            const string RefType = "$ref";

            public bool TreS(string myName111, object node,
                Stack<(string name, object value)> needS,
                List<UnityObjRef> objRefs, Dictionary<object, string> cahce)
            {
                Name = myName111;

                if (node == null)
                {
                    Type = NullType;
                    return true;
                }

                var type = node.GetType();
                Type = type.FullName;

                bool TrySMember(string memberName,
                    object memberValue,
                    Type memberType,
                    out Basic basic)
                {
                    basic = new();

                    basic.Name = memberName;

                    if (typeof(UnityEngine.Object).IsAssignableFrom(memberType))
                    {
                        basic.Type = RefType;
                        var refName = $"{myName111}.{memberName}";
                        basic.Value = refName;

                        UnityObjRef unityObjRef = new();
                        unityObjRef.Name = refName;
                        unityObjRef.Ref = memberValue as UnityEngine.Object;
                        objRefs.Add(unityObjRef);
                    }
                    else if (memberValue == null)
                    {
                        basic.Type = NullType;
                        basic.Value = null;
                    }
                    else
                    {
                        var memberValueType = memberValue.GetType();
                        basic.Type = memberValueType.FullName;
                        if (StringFormatter.TrySerialize(memberValueType, out var destination))
                        {
                            basic.Value = destination;
                        }
                        else
                        {
                            //当作引用序列化
                            if (!cahce.TryGetValue(memberValue, out var refName))
                            {
                                //当前还没有缓存这个引用对象
                                refName = $"{refName}.{memberName}";
                                needS.Push((refName, memberValue));
                            }

                            basic.Type = RefType;
                            basic.Value = refName;
                            //if (memberValueType.IsClass)
                            //{

                            //}
                            //else
                            //{
                            //    Debug.LogError($"myName111.{basic.Name} {basic.Type}序列化失败");
                            //    continue;
                            //}
                        }
                    }

                    return true;
                }

                List<Basic> ms = new();
                if (node is IDictionary dictionary)
                {
                    Debug.LogError($"不支持字典");
                    return false;
                }
                else if (node is IList list)
                {
                    var index = 0;
                    var memberType = type.GetGenericArguments()[0];
                    foreach (var item in list)
                    {
                        if (TrySMember($"Element{index}", item, memberType, out var basic))
                        {
                            ms.Add(basic);
                        }
                        index++;
                    }
                    return true;
                }
                else
                {
                    var members = type.GetFields();
                    foreach (var fieldInfo in members)
                    {
                        var memberValue = fieldInfo.GetValue(node);
                        if (TrySMember(fieldInfo.Name, memberValue, fieldInfo.FieldType, out var basic))
                        {
                            ms.Add(basic);
                        }
                    }
                }

                if (ms.Count > 0)
                {
                    Member = ms;
                }
                return true;
            }


            public int CompareTo(ObjData other)
            {
                return Name.CompareTo(other.Name);
            }
        }
    }
}

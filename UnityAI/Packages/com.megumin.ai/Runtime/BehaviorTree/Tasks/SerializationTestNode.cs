using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Megumin.GameFramework.AI.Serialization;
using Megumin.Serialization;
using UnityEngine;

namespace Megumin.GameFramework.AI.BehaviorTree
{
    [Category("Samples/Serialization")]
    public class SerializationTestNode : ActionTaskNode,
        ISerializationCallbackReceiver<CustomParameterData>,
        ISerializationCallbackReceiver<string>
    {
        [Space]
        public sbyte Sbyte;
        public float Float = 3f;
        public string String = "Hello!";
        public DateTimeOffset DateTimeOffset = DateTimeOffset.Now;
        public Vector2 TestVector2 = Vector2.one;
        public GameObject GameObject;
        public ScriptableObject ScriptableObject;

        [Space]
        public List<int> ListInt = new();
        public List<string> ListString = new();
        public List<GameObject> ListGameObject = new ();

        [Space]
        public float[] ArrayFloat;
        public string[] ArrayString;
        public ScriptableObject[] ArrayScriptableObject;

        [Space]
        public Dictionary<string, int> TestDictionary = new();

        [Space]
        public string CallbackReceiverString;
        public MyClass CallbackReceiverMyClass;


        [Serializable]
        public class MyClass
        {
            public int a;
            public int b;
        }

        public void OnAfterDeserialize(List<CustomParameterData> source)
        {
            foreach (var item in source)
            {
                if (item.MemberName == nameof(CallbackReceiverString))
                {
                    CallbackReceiverString = item.Data;
                }

                if (item.MemberName == nameof(CallbackReceiverMyClass))
                {
                    if (item.Data != null)
                    {
                        CallbackReceiverMyClass = new MyClass();
                        var sp = item.Data.Split("|");
                        if (sp.Length >= 2)
                        {
                            CallbackReceiverMyClass.a = int.Parse(sp[0]);
                            CallbackReceiverMyClass.b = int.Parse(sp[1]);
                        }
                    }
                }
            }
        }

        public void OnBeforeSerialize(List<CustomParameterData> desitination, List<string> ignoreMemberOnSerialize)
        {
            ignoreMemberOnSerialize.Add(nameof(CallbackReceiverString));
            desitination.Add(new CustomParameterData()
            {
                MemberName = nameof(CallbackReceiverString),
                Data = CallbackReceiverString,
            });

            ignoreMemberOnSerialize.Add(nameof(CallbackReceiverMyClass));
            if (CallbackReceiverMyClass != null)
            {
                desitination.Add(new CustomParameterData()
                {
                    MemberName = nameof(CallbackReceiverMyClass),
                    Data = $"{CallbackReceiverMyClass.a}|{CallbackReceiverMyClass.b}",
                });
            }
        }

        public void OnBeforeSerialize(List<string> destination, List<string> ignoreMemberOnSerialize)
        {

        }

        public void OnAfterDeserialize(List<string> source)
        {

        }
    }
}

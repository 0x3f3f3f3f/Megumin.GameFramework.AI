using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Megumin.GameFramework.AI.Serialization;
using UnityEngine;

namespace Megumin.GameFramework.AI.BehaviorTree
{
    [Category("Debug")]
    public class SerializationTestNode : ActionTaskNode,
        ISerializationCallbackReceiver<CustomParameterData>,
        ISerializationCallbackReceiver<string>
    {
        public float TestFloat = 3f;
        public string TestString = "Hello!";
        public DateTimeOffset TestDateTimeOffset = DateTimeOffset.Now;
        public Vector2 TestVector2 = Vector2.one;
        public GameObject TestRef;
        public ScriptableObject TestRefScriptableObject;
        public List<GameObject> TestList = new ();
        public List<int> TestList2 = new();
        public Dictionary<string, int> TestDictionary = new();
        public string[] TestArray;
        public GameObject[] TestRefArray;
        public string TestCallbackReceiver;
        public MyClass TestCallbackReceiverMyClass;

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
                if (item.MemberName == nameof(TestCallbackReceiver))
                {
                    TestCallbackReceiver = item.Value;
                }

                if (item.MemberName == nameof(TestCallbackReceiverMyClass))
                {
                    TestCallbackReceiverMyClass = new MyClass();
                    var sp = item.Value.Split("|");
                    TestCallbackReceiverMyClass.a = int.Parse(sp[0]);
                    TestCallbackReceiverMyClass.b = int.Parse(sp[1]);
                }
            }
        }

        public void OnBeforeSerialize(List<CustomParameterData> desitination, List<string> ignoreMemberOnSerialize)
        {
            ignoreMemberOnSerialize.Add(nameof(TestCallbackReceiver));
            desitination.Add(new CustomParameterData()
            {
                MemberName = nameof(TestCallbackReceiver),
                Value = TestCallbackReceiver,
            });

            ignoreMemberOnSerialize.Add(nameof(TestCallbackReceiverMyClass));
            if (TestCallbackReceiverMyClass != null)
            {
                desitination.Add(new CustomParameterData()
                {
                    MemberName = nameof(TestCallbackReceiverMyClass),
                    Value = $"{TestCallbackReceiverMyClass.a}|{TestCallbackReceiverMyClass.b}",
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

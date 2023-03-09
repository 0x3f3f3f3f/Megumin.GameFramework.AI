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
    [Category("Debug/BTDecorator")]
    internal class SerializationTestDecorator : BTDecorator, ISerializationCallbackReceiver<CustomParameterData>
    {
        public float TestFloat = 3f;
        public GameObject TestRef;
        public List<GameObject> TestList;
        public List<int> TestList2;
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
    }
}

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
                    TestCallbackReceiver = item.Data;
                }

                if (item.MemberName == nameof(TestCallbackReceiverMyClass))
                {
                    TestCallbackReceiverMyClass = new MyClass();
                    var sp = item.Data.Split("|");
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
                Data = TestCallbackReceiver,
            });

            ignoreMemberOnSerialize.Add(nameof(TestCallbackReceiverMyClass));
            if (TestCallbackReceiverMyClass != null)
            {
                desitination.Add(new CustomParameterData()
                {
                    MemberName = nameof(TestCallbackReceiverMyClass),
                    Data = $"{TestCallbackReceiverMyClass.a}|{TestCallbackReceiverMyClass.b}",
                });
            }
        }
    }
}

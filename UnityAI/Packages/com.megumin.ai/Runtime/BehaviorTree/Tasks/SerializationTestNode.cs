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
        ISerializationCallbackReceiver<CollectionSerializationData>,
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

        [Space]
        //[SerializeReference]
        public TestVariable ExposeString;// = new ParamVariable_string() { Name = "test1",Value ="hello",Path = "GameObject/tag" };
        public ParamVariable<GameObject> ExposeGameObject;
        public MMData<string> MMDataInt;
        public MMData<GameObject> MMDataGameObject;

        [Serializable]
        public class MyClass
        {
            public int a;
            public int b;
        }

        public void OnAfterDeserialize(List<CollectionSerializationData> source)
        {
            foreach (var item in source)
            {
                if (item.Name == nameof(CallbackReceiverString))
                {
                    CallbackReceiverString = item.Data;
                }

                if (item.Name == nameof(CallbackReceiverMyClass))
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

        public void OnBeforeSerialize(List<CollectionSerializationData> desitination, List<string> ignoreMemberOnSerialize)
        {
            ignoreMemberOnSerialize.Add(nameof(CallbackReceiverString));
            desitination.Add(new CollectionSerializationData()
            {
                Name = nameof(CallbackReceiverString),
                Data = CallbackReceiverString,
            });

            ignoreMemberOnSerialize.Add(nameof(CallbackReceiverMyClass));
            if (CallbackReceiverMyClass != null)
            {
                desitination.Add(new CollectionSerializationData()
                {
                    Name = nameof(CallbackReceiverMyClass),
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

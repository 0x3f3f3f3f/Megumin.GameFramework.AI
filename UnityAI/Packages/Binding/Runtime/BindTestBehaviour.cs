using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Megumin.Binding
{
    public class BindTestBehaviour : MonoBehaviour
    {
        /// 2023 及以后版本没有泛型限制。
        /// [SerializeReference]不支持泛型，无论实例类型是泛型，还是标记类型是泛型，都不能支持。
        /// A class derived from a generic type, but not a specific specialization of a generic type (inflated type). For example, you can't use the [SerializeReference] attribute with the type , instead you must create a non-generic subclass of your generic instance type and use that as the field type instead, like this:
        /// 
        /// 
        /// 
        /// 
        /// 
        public BindingsSO TestSO;
        public SerializeValue Test11;
        public BindableIntValue Int11;
        public BindableIntValue Int22;
        public BindableValue<int> G111;
        public BindableValue<string> GString222;
        public BindableValue<DateTime> G333;
        public BindableValue<float> GFloat444;

        //[SerializeReference]
        //public TestGAB<int> G222 = new TestG<int>();
        [SerializeReference]
        public List<BindableIntValue> IBindables = new List<BindableIntValue>();
        //[SerializeReference]
        //public IData<int> Test222 = new BindableIntValue();

        [SerializeReference]
        public List<IData> InterfaceTest = new List<IData>()
        {
            new BindableIntValue(),
        };

        [Button]
        public void AddMiss()
        {
            IBindables.Clear();
            IBindables.Add(new BindableIntValue() { Key = nameof(TestSO.TestBindingInt) });
            IBindables.Add(new BindableIntValue() { Key = nameof(TestSO.MyStrName) });
            IBindables.Add(new BindableIntValue() { Key = nameof(TestSO.MyGameObject) });
        }

        [Button]
        public void Parse()
        {
            //var b = TestSO.BindInt;
            //b.InitializeBinding(gameObject);
            //Debug.Log(b.Value);

            //GString222.InitializeBinding(gameObject);
            //Debug.Log(GString222.Value);
            Debug.Log(Time.fixedDeltaTime);
            var f = GFloat444;
            f.InitializeBinding(gameObject);
            Debug.Log(f.Value);
        }

#if UNITY_2023_1_OR_NEWER

        [Header("UNITY_2023_1_OR_NEWER  SerializeReference 泛型特化支持")]
        [SerializeReference]
        public IData mydata1 = new BindableIntValue();

        [SerializeReference]
        public IData<int> mydata2 = new BindableIntValue();

        [SerializeReference]
        public IData<int> mydata3 = new BindableValue<int>();

        [SerializeReference]
        public IData mydata4 = new BindableValue<int>();

        [SerializeReference]
        public List<IData> DatasList1 = new List<IData>()
        {
            new BindableIntValue(){ Value = 101},
            new BindableValue<int>{ Value = 102},
            new BindableValue<string>{Value = "MydataList_102"}
        };

        [SerializeReference]
        public List<IData<int>> DatasList2 = new List<IData<int>>()
        {
            new BindableIntValue(){ Value = 101},
            new BindableValue<int>{ Value = 102},
        };

#endif
    }
}




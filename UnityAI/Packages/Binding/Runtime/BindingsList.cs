using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Megumin;
using System.Runtime.Serialization;
using System;

namespace Megumin.Binding
{
    public class BindingsList : MonoBehaviour
    {

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
            var b = TestSO.BindInt;
            b = Int11;
            var bstr = b.BindingString;
            b.InitializeBinding(gameObject);
            Debug.Log(b.Value);

            //GString222.InitializeBinding(gameObject);
            //Debug.Log(GString222.Value);

            var f = GFloat444;
            f.InitializeBinding(gameObject);
            Debug.Log(f.Value);
        }
    }
}

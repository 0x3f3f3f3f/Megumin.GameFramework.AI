using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Megumin;
using System.Runtime.Serialization;

namespace Megumin.Binding
{
    public class BindingsList : MonoBehaviour
    {
        public BindingsSO TestSO;
        public SerializeValue Test11;
        public BindableIntValue Int11;
        public BindableIntValue Int22;

        [SerializeReference]
        public List<BindableIntValue> IBindables=new List<BindableIntValue>();

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
        }
    }
}

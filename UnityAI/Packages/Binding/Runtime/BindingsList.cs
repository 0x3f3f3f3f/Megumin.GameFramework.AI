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

        [SerializeReference]
        public List<SerializeValue> IBindables=new List<SerializeValue>();

        [Button]
        public void AddMiss()
        {
            IBindables.Clear();
            IBindables.Add(new SerializeValue() { Key = nameof(TestSO.TestBindingInt) });
            IBindables.Add(new SerializeValue() { Key = nameof(TestSO.MyStrName) });
            IBindables.Add(new SerializeValue() { Key = nameof(TestSO.MyGameObject) });
        }

        [Button]
        public void Parse()
        {
            var b = TestSO.BindInt;
            var bstr = b.BindingString;
            b.InitializeBinding(gameObject);
            Debug.Log(b.Value);
        }
    }
}

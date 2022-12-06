using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Megumin.Binding
{
    public class BindingsSO : ScriptableObject
    {
        public IData<int> TestBindingInt;
        public IData<string> MyStrName;
        public IData<GameObject> MyGameObject;

        public BindableIntValue BindInt;
        public string BindStr;


        [Button]
        public void TestLog()
        {
            Debug.Log($"{TestBindingInt.Value}--{MyStrName.Value}--{MyGameObject.Value}");
        }
    }
}

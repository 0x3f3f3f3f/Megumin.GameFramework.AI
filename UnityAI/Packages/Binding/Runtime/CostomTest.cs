using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Megumin.Binding
{
    public class CostomTest : MonoBehaviour
    {
        public int MyIntField1 = 100;
        public int MyIntField2 = 200;

        [field: SerializeField]
        public int MyIntProperty1 { get; set; } = 100;
        public int MyIntProperty2 => MyIntProperty2;

        public int MyIntMethod1()
        {
            return MyIntField1;
        }

        public int MyIntMethod2(GameObject game)
        {
            return MyIntField2;
        }


        public string MystringField1 = "HelloWorld1";
        public string MystringField2 = "HelloWorld2";

        [field: SerializeField]
        public string MystringProperty1 { get; set; } = "MystringPropertyHelloWorld1";
        public string MystringProperty2 => MystringProperty2;

        public string MystringMethod1()
        {
            return MystringField1;
        }

        public string MystringMethod2(GameObject game)
        {
            return MystringField2;
        }
    }
}

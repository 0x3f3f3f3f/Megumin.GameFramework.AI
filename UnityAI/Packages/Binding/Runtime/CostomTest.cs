using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Megumin.Binding
{
    public interface ICostomTestInterface
    {
        int MyIntProperty1 { get; set; }
        int MyIntProperty2 { get; }
        string MystringProperty1 { get; set; }
        string MystringProperty2 { get; }

        int MyIntMethod1();
        int MyIntMethod2(GameObject game);
        string MystringMethod1();
        string MystringMethod2(GameObject game);
    }

    public interface ICostomTestInterface222
    {

    }

    public class CostomTestClass : ICostomTestInterface222
    {

    }

    public class CostomTest : MonoBehaviour, ICostomTestInterface
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

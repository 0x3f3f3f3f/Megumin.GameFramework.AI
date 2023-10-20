using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using UnityEngine;

namespace Megumin.Binding.Test
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

    public interface ICostomTestInterface2
    {

    }

    public class CostomTestClass : ICostomTestInterface2
    {

    }



    public class CostomTest : MonoBehaviour, ICostomTestInterface
    {
        public int MyIntField1 = 100;
        public int MyIntField2 = 200;

        [field: SerializeField]
        public int MyIntProperty1 { get; set; } = 100;
        public int MyIntProperty2 => MyIntProperty2;

        public List<int> ListIntTest = new List<int>() { 15, 25, 35 };
        public List<string> ListStringTest = new List<string>() { "Tom", "Jerry" };
        public Dictionary<int, string> Dic_Int_StringTest = new Dictionary<int, string>()
        {
            { 0, "Hello"},
            { 1, "Goodmorning" },
            { 25,"Bye" },
        };

        public Dictionary<string, int> Dic_String_IntTest = new Dictionary<string, int>()
        {
            { "a", 100 },
            { "b", 200 },
        };

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

        public Type TypeProperty1 { get; set; } = typeof(System.Tuple<int, string>);

        public string MystringMethod1()
        {
            return MystringField1;
        }

        public string MystringMethod2(GameObject game)
        {
            return MystringField2;
        }

        public void MystringMethodSet(string str)
        {
            MystringField2 = str;
        }

        public string MystringMethodSetReturnString(string str)
        {
            MystringField2 = str;
            return MystringField2;
        }

        public TestInnerClass MyTestInnerClassField = new TestInnerClass();
    }

    public class TestInnerClass
    {
        public int MyIntField1 = 100;
        public int MyIntField2 = 200;


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

        public int this[int index]
        {
            get { return index * 100; }
            set { }
        }

        /// <summary>
        /// 索引器重载测试
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public int this[string index]
        {
            get { return 9999; }
            set { }
        }

        /// <summary>
        /// 多个参数索引器测试
        /// </summary>
        /// <param name="key1"></param>
        /// <param name="key2"></param>
        /// <returns></returns>
        public int this[int key1, string key2]
        {
            get { return 1000; }
            set { }
        }

        public string MystringField1 = "TestInnerClass_HelloWorld1";
        public string MystringField2 = "TestInnerClass_HelloWorld2";

        [field: SerializeField]
        public string MystringProperty1 { get; set; } = "MystringPropertyHelloWorld1";
        public string MystringProperty2 => MystringProperty2;

        public TestInnerClassDeep2 MyTestInnerClassDeep2 { get; set; } = new TestInnerClassDeep2();
    }

    public class TestInnerClassDeep2
    {
        public int MyIntField1 = 100;
        public int MyIntField2 = 200;


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


        public string MystringField1 = "TestInnerClassDeep2_HelloWorld1";
        public string MystringField2 = "TestInnerClassDeep2_HelloWorld2";

        [field: SerializeField]
        public string MystringProperty1 { get; set; } = "MystringProperty TestInnerClassDeep2 HelloWorld1";
        public string MystringProperty2 => MystringProperty2;
    }
}

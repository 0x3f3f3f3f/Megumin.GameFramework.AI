using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Megumin.Reflection;
using UnityEngine;

namespace Megumin.Binding.Test
{
    public class BindTestBehaviour : MonoBehaviour
    {
        /// 2023 及以后版本没有泛型限制。
        /// [SerializeReference]不支持泛型，无论实例类型是泛型，还是标记类型是泛型，都不能支持。
        /// A class derived from a generic type, but not a specific specialization of a generic type (inflated type). For example, you can't use the [SerializeReference] attribute with the type , instead you must create a non-generic subclass of your generic instance type and use that as the field type instead, like this:
        /// 

        public BindingsSO TestSO;

        /// <summary>
        /// 属性绑定 ✅
        /// </summary>
        public BindingVar<int> TestInt
            = new() { BindingPath = "UnityEngine.GameObject/layer" };

        /// <summary>
        /// 属性绑定 ✅
        /// </summary>
        public BindingVar<string> GameObjectTag
            = new() { BindingPath = "UnityEngine.GameObject/tag" };

        /// <summary>
        /// 类型自动适配，自动转型 ✅
        /// </summary>
        public BindingVar<object> TypeAdpterTestString2Object
            = new() { BindingPath = "UnityEngine.GameObject/tag" };

        /// <summary>
        /// 类型自动适配，自动转型 ✅
        /// </summary>
        public BindingVar<object> TypeAdpterTestInt2Object
            = new() { BindingPath = "UnityEngine.GameObject/layer" };

        /// <summary>
        /// 类型自动适配，自动转型 ✅
        /// </summary>
        public BindingVar<string> TypeAdpterTestInt2String
            = new() { BindingPath = "UnityEngine.GameObject/layer" };

        /// <summary>
        /// 类型自动适配，自动转型 ✅
        /// </summary>
        public BindingVar<float> TypeAdpterTestInt2Float
            = new() { BindingPath = "UnityEngine.GameObject/layer" };

        /// <summary>
        /// 类型自动适配，自动转型 ✅
        /// </summary>
        public BindingVar<string> TypeAdpterTestGameObject2String
            = new() { BindingPath = "UnityEngine.Transform/gameObject" };

        /// <summary>
        /// 类型自动适配，自动转型 ✅
        /// </summary>
        public BindingVar<string> TypeAdpterTestTestInnerClass2String
            = new() { BindingPath = "Megumin.Binding.Test.ICostomTestInterface/MyTestInnerClassField" };

        /// <summary>
        /// 字段绑定 ✅
        /// </summary>
        public BindingVar<string> CustomTestField
            = new()
            {
                value = "MathFailure",
                BindingPath = "Megumin.Binding.Test.CostomTest/MystringField1"
            };

        /// <summary>
        /// 接口字段绑定。接口是用来取得Component的，后续字符串成员不一定时接口的成员。 ✅
        /// </summary>
        public BindingVar<string> CustomTestFieldByInterface
            = new()
            {
                value = "MathFailure_CustomTestFieldByInterface",
                BindingPath = "Megumin.Binding.Test.ICostomTestInterface/MystringField1"
            };

        /// <summary>
        /// 接口字段绑定。测试绑定为接口但是无法找到组件。 预期结果： 无法解析，但是不能造成崩溃。 ✅
        /// </summary>
        public BindingVar<string> CustomTestFieldByInterface2
            = new()
            {
                value = "MathFailure_CustomTestFieldByInterface2",
                BindingPath = "Megumin.Binding.Test.ICostomTestInterface2/MystringField1"
            };

        /// <summary>
        /// 接口字段绑定。测试绑定为非组件非静态类型。 预期结果： 无法解析，但是不能造成崩溃。 ✅
        /// </summary>
        public BindingVar<string> CustomTestFieldByCostomTestClass
            = new()
            {
                value = "MathFailure_CostomTestClass",
                BindingPath = "Megumin.Binding.Test.CostomTestClass/MystringField1"
            };

        /// <summary>
        /// 多级成员绑定 ✅
        /// </summary>
        public BindingVar<string> GameObjectTransformTag
            = new() { BindingPath = "UnityEngine.GameObject/transform/tag" };

        /// <summary>
        /// 多级成员绑定 ✅
        /// </summary>
        public BindingVar<string> MyTestInnerClass
            = new() { BindingPath = "Megumin.Binding.Test.ICostomTestInterface/MyTestInnerClassField/MystringField1" };

        /// <summary>
        /// 索引器绑定 ✅
        /// </summary>
        public BindingVar<int> MyTestIndexer
            = new() { BindingPath = "Megumin.Binding.Test.ICostomTestInterface/MyTestInnerClassField[10]" };

        /// <summary>
        /// Array索引器绑定 ✅
        /// </summary>
        public BindingVar<int> ArrayIntIndexer
            = new() { BindingPath = "Megumin.Binding.Test.ICostomTestInterface/ArrayIntTest[1]" };

        /// <summary>
        /// Array索引器绑定 ✅
        /// </summary>
        public BindingVar<string> ArrayStringIndexer
            = new() { BindingPath = "Megumin.Binding.Test.ICostomTestInterface/ArrayStringTest[1]" };

        /// <summary>
        /// List索引器绑定 ✅
        /// </summary>
        public BindingVar<int> ListIntIndexer
            = new() { BindingPath = "Megumin.Binding.Test.ICostomTestInterface/ListIntTest[1]" };

        /// <summary>
        /// List索引器绑定 ✅
        /// </summary>
        public BindingVar<string> ListStringIndexer
            = new() { BindingPath = "Megumin.Binding.Test.ICostomTestInterface/ListStringTest[1]" };

        /// <summary>
        /// 字典索引器绑定 ✅
        /// </summary>
        public BindingVar<int> DicIntIndexer
            = new() { BindingPath = "Megumin.Binding.Test.ICostomTestInterface/Dic_String_IntTest[b]" };

        /// <summary>
        /// 字典索引器绑定 ✅
        /// </summary>
        public BindingVar<string> DicStringIndexer
            = new() { BindingPath = "Megumin.Binding.Test.ICostomTestInterface/Dic_Int_StringTest[1]" };

        /// <summary>
        /// 多级成员绑定 ✅
        /// </summary>
        public BindingVar<string> MyTestInnerClassDeep2
            = new() { BindingPath = "Megumin.Binding.Test.ICostomTestInterface/MyTestInnerClassField/MyTestInnerClassDeep2/MystringField1" };


        /// <summary>
        /// 静态类型绑定 ✅
        /// </summary>
        public BindingVar<string> ApplicationVersion
            = new() { BindingPath = "UnityEngine.Application/version" };

        /// <summary>
        /// 静态类型绑定 ✅
        /// </summary>
        public BindingVar<float> TimeFixedDeltaTime
            = new() { BindingPath = "UnityEngine.Time/fixedDeltaTime" };

        /// <summary>
        /// 绑定非序列化类型 ✅
        /// </summary>
        public BindingVar<DateTimeOffset> DateTimeOffsetOffset
            = new()
            {
                value = new DateTimeOffset(2000, 1, 1, 0, 0, 0, default),
                BindingPath = "System.DateTimeOffset/Now",
            };

        /// <summary>
        /// 绑定非序列化类型 ✅
        /// </summary>
        public BindingVar<Type> BindType
            = new()
            {
                value = typeof(System.Version),
                BindingPath = "System.DateTimeOffset",
            };

        /// <summary>
        /// 绑定非序列化类型 ✅
        /// </summary>
        public BindingVar<Type> BindTypeProperty
            = new()
            {
                value = typeof(System.Version),
                BindingPath = "Megumin.Binding.Test.ICostomTestInterface/TypeProperty1",
            };

        /// <summary>
        /// 绑定方法（0个参数，或者1个参数的某些特殊方法） ✅
        /// </summary>
        public BindingVar<string> BindMethodArgs0
            = new()
            {
                value = "MathFailure_CustomTestFieldByInterface_BindMethodArgs0",
                BindingPath = "Megumin.Binding.Test.ICostomTestInterface/MystringMethod1()"
            };

        /// <summary>
        /// 绑定方法（0个参数，或者1个参数的某些特殊方法） ✅
        /// </summary>
        public BindingVar<string> BindMethodArgs1
            = new()
            {
                value = "MathFailure_CustomTestFieldByInterface_BindMethodArgs1",
                BindingPath = "Megumin.Binding.Test.ICostomTestInterface/MystringMethod2()"
            };

        /// <summary>
        /// 绑定方法（0个参数，或者1个参数的某些特殊方法） ✅
        /// </summary>
        public BindingVar<string> BindMethodArgs1Set
            = new()
            {
                value = "MathFailure_CustomTestFieldByInterface_BindMethodArgs1Set",
                BindingPath = "Megumin.Binding.Test.ICostomTestInterface/MystringMethodSet()"
            };

        /// <summary>
        /// 绑定方法（0个参数，或者1个参数的某些特殊方法） ✅
        /// </summary>
        public BindingVar<string> BindMethodArgs1SetReturnString
            = new()
            {
                value = "MathFailure_CustomTestFieldByInterface_BindMethodArgs1SetReturnString",
                BindingPath = "Megumin.Binding.Test.ICostomTestInterface/MystringMethodSetReturnString()"
            };

        /// <summary>
        /// 绑定方法（0个参数，或者1个参数的某些特殊方法） ✅
        /// </summary>
        public BindingVar<string> Test1
            = new() { BindingPath = "UnityEngine.GameObject/ToString()" };

        /// <summary>
        /// 绑定泛型方法 TODO
        /// </summary>
        public BindingVar<string> Test2
            = new() { BindingPath = "UnityEngine.Application/version" };

        /// <summary>
        /// 绑定扩展方法 TODO
        /// </summary>
        public BindingVar<string> Test3
           = new() { BindingPath = "UnityEngine.Application/version" };

        [SerializeReference]
        internal List<BindableValueInt> IBindables = new();

        [SerializeReference]
        public List<IVariable> InterfaceTest = new()
        {
            new BindableValueInt() { BindingPath = "UnityEngine.GameObject/layer" },
            new BindableValueString() { BindingPath = "UnityEngine.GameObject/tag" },
        };

        [ContextMenu(nameof(AddMiss))]
        [Editor]
        public void AddMiss()
        {
            IBindables.Clear();
            IBindables.Add(new BindableValueInt() { Key = nameof(TestSO.NeedOverrideInt1) });
            IBindables.Add(new BindableValueInt() { Key = nameof(TestSO.NeedOverrideInt2) });
            IBindables.Add(new BindableValueInt() { Key = nameof(TestSO.NeedOverrideInt3) });
        }

        [Editor]
        public void SetValueTest()
        {
            var f = MyTestInnerClass;
            f.ParseBinding(gameObject, true);
            f.Value = "Finish";
            Debug.Log($"{f.BindingPath}   {f.Value}");
        }

        public void AOT()
        {
            //注意 成员很可能被IL2CPP剪裁掉导致无法绑定。
            Debug.Log(Application.version);
            Debug.Log(Time.time);
            Debug.Log(DateTimeOffset.Now);
        }

        [Editor]
        public void Test()
        {
            ReflectionExtension_9C4E15F3B30F4FCFBC57EDC2A99A69D0.TestConvert();
            {
                var type = typeof(Application);
                var prop = type.GetProperty("version");
                if (prop.GetMethod.TryCreateGetter(type, null, out var @delegate, false))
                {
                    Debug.Log(@delegate.DynamicInvoke());
                }

                if (prop.TryCreateGetter<string>(type, null, out var getter))
                {
                    Debug.Log(getter());
                }
            }

            {
                var obj = GetComponent<CostomTest>();
                var prop2 = obj.GetType().GetProperty("MyIntProperty1");
                if (prop2.GetMethod.TryCreateGetter(obj.GetType(), obj, out var @delegate2, false))
                {
                    Debug.Log(@delegate2.DynamicInvoke());
                }
            }
        }

#if UNITY_2023_1_OR_NEWER

        [Header("UNITY_2023_1_OR_NEWER  SerializeReference 泛型特化支持")]
        [SerializeReference]
        public IVariable mydata1 = new BindableValueInt();

        [SerializeReference]
        public IVariable<int> mydata2 = new BindableValueInt();

        [SerializeReference]
        public IVariable<int> mydata3 = new BindingVar<int>();

        [SerializeReference]
        public IVariable mydata4 = new BindingVar<int>();

        [SerializeReference]
        public List<IVariable> DatasList1 = new List<IVariable>()
        {
            new BindableValueInt(){ value = 101},
            new BindingVar<int>{ value = 102},
            new BindingVar<string>{value = "MydataList_102"}
        };

        [SerializeReference]
        public List<IVariable<int>> DatasList2 = new List<IVariable<int>>()
        {
            new BindableValueInt(){ value = 101},
            new BindingVar<int>{ value = 102},
        };

#endif

        private string debugString;
        private void OnGUI()
        {
            ///打包测试


            GUILayout.BeginArea(new Rect(100, Screen.height / 2, Screen.width - 200, Screen.height / 2));
            GUILayout.Label($"DebugString  :  {debugString}", GUILayout.ExpandWidth(true));

            List<(IBindingParseable, string Name)> testBind = new();

            var fields = this.GetType().GetFields();

            foreach (var field in fields)
            {
                if (typeof(IBindingParseable).IsAssignableFrom(field.FieldType))
                {
                    var p = (IBindingParseable)field.GetValue(this);
                    testBind.Add((p, field.Name));

                    //if (GUILayout.Button(field.Name))
                    //{
                    //    p.ParseBinding(gameObject, true);
                    //    debugString = p.DebugParseResult();
                    //}
                }
            }

            var properties = this.GetType().GetProperties();

            foreach (var property in properties)
            {
                if (typeof(IBindingParseable).IsAssignableFrom(property.PropertyType))
                {
                    var p = (IBindingParseable)property.GetValue(this);
                    testBind.Add((p, property.Name));
                    //if (GUILayout.Button(property.Name))
                    //{
                    //    p.ParseBinding(gameObject, true);
                    //    debugString = p.DebugParseResult();
                    //}
                }
            }


            GUILayout.BeginHorizontal();

            int index = 0;
            int bottonWidth = 300;
            int parLine = (Screen.width - 200) / bottonWidth;
            if (parLine == 0)
            {
                parLine = 1;
            }

            foreach (var item in testBind)
            {
                if (GUILayout.Button(item.Name,
                    GUILayout.Width(bottonWidth),
                    GUILayout.Height(40)))
                {
                    var p = item.Item1;
                    p.ParseBinding(gameObject, true);
                    debugString = p.DebugParseResult();
                }
                index++;
                if (index % parLine == 0)
                {
                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal();
                }
            }

            GUILayout.EndHorizontal();
            GUILayout.EndArea();
        }
    }
}




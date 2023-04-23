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
        public BindableValue<int> TestInt
            = new BindableValue<int>() { BindingPath = "UnityEngine.GameObject/layer" };

        /// <summary>
        /// 属性绑定 ✅
        /// </summary>
        public BindableValue<string> GameObjectTag
            = new BindableValue<string>() { BindingPath = "UnityEngine.GameObject/tag" };

        /// <summary>
        /// 类型自动适配，自动转型 ✅
        /// </summary>
        public BindableValue<object> TypeAdpterTestString2Object
            = new BindableValue<object>() { BindingPath = "UnityEngine.GameObject/tag" };

        /// <summary>
        /// 类型自动适配，自动转型 ✅
        /// </summary>
        public BindableValue<object> TypeAdpterTestInt2Object
            = new BindableValue<object>() { BindingPath = "UnityEngine.GameObject/layer" };

        /// <summary>
        /// 类型自动适配，自动转型 ✅
        /// </summary>
        public BindableValue<string> TypeAdpterTestInt2String
            = new BindableValue<string>() { BindingPath = "UnityEngine.GameObject/layer" };

        /// <summary>
        /// 类型自动适配，自动转型 ✅
        /// </summary>
        public BindableValue<float> TypeAdpterTestInt2Float
            = new BindableValue<float>() { BindingPath = "UnityEngine.GameObject/layer" };

        /// <summary>
        /// 类型自动适配，自动转型 ✅
        /// </summary>
        public BindableValue<string> TypeAdpterTestGameObject2String
            = new BindableValue<string>() { BindingPath = "UnityEngine.Transform/gameObject" };

        /// <summary>
        /// 类型自动适配，自动转型 ✅
        /// </summary>
        public BindableValue<string> TypeAdpterTestTestInnerClass2String
            = new BindableValue<string>() { BindingPath = "Megumin.Binding.Test.ICostomTestInterface/MyTestInnerClassField" };

        /// <summary>
        /// 字段绑定 ✅
        /// </summary>
        public BindableValue<string> CustomTestField
            = new BindableValue<string>()
            {
                DefaultValue = "MathFailure",
                BindingPath = "Megumin.Binding.Test.CostomTest/MystringField1"
            };

        /// <summary>
        /// 接口字段绑定。接口是用来取得Component的，后续字符串成员不一定时接口的成员。 ✅
        /// </summary>
        public BindableValue<string> CustomTestFieldByInterface
            = new BindableValue<string>()
            {
                DefaultValue = "MathFailure_CustomTestFieldByInterface",
                BindingPath = "Megumin.Binding.Test.ICostomTestInterface/MystringField1"
            };

        /// <summary>
        /// 接口字段绑定。测试绑定为接口但是无法找到组件。 预期结果： 无法解析，但是不能造成崩溃。 ✅
        /// </summary>
        public BindableValue<string> CustomTestFieldByInterface2
            = new BindableValue<string>()
            {
                DefaultValue = "MathFailure_CustomTestFieldByInterface2",
                BindingPath = "Megumin.Binding.Test.ICostomTestInterface2/MystringField1"
            };

        /// <summary>
        /// 接口字段绑定。测试绑定为非组件非静态类型。 预期结果： 无法解析，但是不能造成崩溃。 ✅
        /// </summary>
        public BindableValue<string> CustomTestFieldByCostomTestClass
            = new BindableValue<string>()
            {
                DefaultValue = "MathFailure_CostomTestClass",
                BindingPath = "Megumin.Binding.Test.CostomTestClass/MystringField1"
            };

        /// <summary>
        /// 多级成员绑定 ✅
        /// </summary>
        public BindableValue<string> GameObjectTransformTag
            = new BindableValue<string>() { BindingPath = "UnityEngine.GameObject/transform/tag" };

        /// <summary>
        /// 多级成员绑定 ✅
        /// </summary>
        public BindableValue<string> MyTestInnerClass
            = new BindableValue<string>() { BindingPath = "Megumin.Binding.Test.ICostomTestInterface/MyTestInnerClassField/MystringField1" };

        /// <summary>
        /// 多级成员绑定 ✅
        /// </summary>
        public BindableValue<string> MyTestInnerClassDeep2
            = new BindableValue<string>() { BindingPath = "Megumin.Binding.Test.ICostomTestInterface/MyTestInnerClassField/MyTestInnerClassDeep2/MystringField1" };

        /// <summary>
        /// 静态类型绑定 ✅
        /// </summary>
        public BindableValue<string> ApplicationVersion
            = new BindableValue<string>() { BindingPath = "UnityEngine.Application/version" };

        /// <summary>
        /// 静态类型绑定 ✅
        /// </summary>
        public BindableValue<float> TimeFixedDeltaTime
            = new BindableValue<float>() { BindingPath = "UnityEngine.Time/fixedDeltaTime" };

        /// <summary>
        /// 绑定非序列化类型 ✅
        /// </summary>
        public BindableValue<DateTimeOffset> DateTimeOffsetOffset
            = new BindableValue<DateTimeOffset>()
            {
                DefaultValue = new DateTimeOffset(2000, 1, 1, 0, 0, 0, default),
                BindingPath = "System.DateTimeOffset/Now",
            };

        /// <summary>
        /// 绑定非序列化类型 ✅
        /// </summary>
        public BindableValue<Type> BindType
            = new BindableValue<Type>()
            {
                DefaultValue = typeof(System.Version),
                BindingPath = "System.DateTimeOffset",
            };

        /// <summary>
        /// 绑定非序列化类型 ✅
        /// </summary>
        public BindableValue<Type> BindTypeProperty
            = new BindableValue<Type>()
            {
                DefaultValue = typeof(System.Version),
                BindingPath = "Megumin.Binding.Test.ICostomTestInterface/TypeProperty1",
            };

        /// <summary>
        /// 绑定方法（0个参数，或者1个参数的某些特殊方法） ✅
        /// </summary>
        public BindableValue<string> BindMethodArgs0
            = new BindableValue<string>()
            {
                DefaultValue = "MathFailure_CustomTestFieldByInterface_BindMethodArgs0",
                BindingPath = "Megumin.Binding.Test.ICostomTestInterface/MystringMethod1()"
            };

        /// <summary>
        /// 绑定方法（0个参数，或者1个参数的某些特殊方法） ✅
        /// </summary>
        public BindableValue<string> BindMethodArgs1
            = new BindableValue<string>()
            {
                DefaultValue = "MathFailure_CustomTestFieldByInterface_BindMethodArgs1",
                BindingPath = "Megumin.Binding.Test.ICostomTestInterface/MystringMethod2()"
            };

        /// <summary>
        /// 绑定方法（0个参数，或者1个参数的某些特殊方法） ✅
        /// </summary>
        public BindableValue<string> BindMethodArgs1Set
            = new BindableValue<string>()
            {
                DefaultValue = "MathFailure_CustomTestFieldByInterface_BindMethodArgs1Set",
                BindingPath = "Megumin.Binding.Test.ICostomTestInterface/MystringMethodSet()"
            };

        /// <summary>
        /// 绑定方法（0个参数，或者1个参数的某些特殊方法） ✅
        /// </summary>
        public BindableValue<string> BindMethodArgs1SetReturnString
            = new BindableValue<string>()
            {
                DefaultValue = "MathFailure_CustomTestFieldByInterface_BindMethodArgs1SetReturnString",
                BindingPath = "Megumin.Binding.Test.ICostomTestInterface/MystringMethodSetReturnString()"
            };

        /// <summary>
        /// 绑定方法（0个参数，或者1个参数的某些特殊方法） ✅
        /// </summary>
        public BindableValue<string> Test1
            = new BindableValue<string>() { BindingPath = "UnityEngine.GameObject/ToString()" };

        /// <summary>
        /// 绑定泛型方法 TODO
        /// </summary>
        public BindableValue<string> Test2
            = new BindableValue<string>() { BindingPath = "UnityEngine.Application/version" };

        /// <summary>
        /// 绑定扩展方法 TODO
        /// </summary>
        public BindableValue<string> Test3
           = new BindableValue<string>() { BindingPath = "UnityEngine.Application/version" };

        [SerializeReference]
        public List<BindableValueInt> IBindables = new List<BindableValueInt>();

        [SerializeReference]
        public List<IVariable> InterfaceTest = new List<IVariable>()
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
        public IVariable<int> mydata3 = new BindableValue<int>();

        [SerializeReference]
        public IVariable mydata4 = new BindableValue<int>();

        [SerializeReference]
        public List<IVariable> DatasList1 = new List<IVariable>()
        {
            new BindableValueInt(){ Value = 101},
            new BindableValue<int>{ Value = 102},
            new BindableValue<string>{Value = "MydataList_102"}
        };

        [SerializeReference]
        public List<IVariable<int>> DatasList2 = new List<IVariable<int>>()
        {
            new BindableValueInt(){ Value = 101},
            new BindableValue<int>{ Value = 102},
        };

#endif

        private string debugString;
        private void OnGUI()
        {
            ///打包测试


            GUILayout.BeginArea(new Rect(100, Screen.height / 2, Screen.width - 200, Screen.height / 2));
            GUILayout.Label($"DebugString  :  {debugString}", GUILayout.ExpandWidth(true));

            List<(IBindingParseable, string Name)> testBind = new List<(IBindingParseable, string Name)>();

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




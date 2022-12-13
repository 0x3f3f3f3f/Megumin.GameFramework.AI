using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

namespace Megumin.Binding
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
        public BindableValue<string> GameObjectTag
            = new BindableValue<string>() { BindingPath = "UnityEngine.GameObject/tag" };

        /// <summary>
        /// 类型自动适配，自动转型
        /// </summary>
        public BindableValue<string> TypeAdpterTest
            = new BindableValue<string>() { BindingPath = "UnityEngine.GameObject/layer" };

        /// <summary>
        /// 字段绑定 ✅
        /// </summary>
        public BindableValue<string> CustomTestField
            = new BindableValue<string>()
            {
                DefaultValue = "MathFailure",
                BindingPath = "Megumin.Binding.CostomTest/MystringField1"
            };

        /// <summary>
        /// 接口字段绑定。接口是用来取得Component的，后续字符串成员不一定时接口的成员。 ✅
        /// </summary>
        public BindableValue<string> CustomTestFieldByInterface
            = new BindableValue<string>()
            {
                DefaultValue = "MathFailure_CustomTestFieldByInterface",
                BindingPath = "Megumin.Binding.ICostomTestInterface/MystringField1"
            };

        /// <summary>
        /// 接口字段绑定。测试绑定为接口但是无法找到组件。 预期结果： 无法解析，但是不能造成崩溃。 ✅
        /// </summary>
        public BindableValue<string> CustomTestFieldByInterface2
            = new BindableValue<string>()
            {
                DefaultValue = "MathFailure_CustomTestFieldByInterface2",
                BindingPath = "Megumin.Binding.ICostomTestInterface2/MystringField1"
            };

        /// <summary>
        /// 接口字段绑定。测试绑定为非组件非静态类型。 预期结果： 无法解析，但是不能造成崩溃。 ✅
        /// </summary>
        public BindableValue<string> CustomTestFieldByCostomTestClass
            = new BindableValue<string>()
            {
                DefaultValue = "MathFailure_CostomTestClass",
                BindingPath = "Megumin.Binding.CostomTestClass/MystringField1"
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
            = new BindableValue<string>() { BindingPath = "Megumin.Binding.ICostomTestInterface/MyTestInnerClassField/MystringField1" };

        /// <summary>
        /// 多级成员绑定 ✅
        /// </summary>
        public BindableValue<string> MyTestInnerClassDeep2
            = new BindableValue<string>() { BindingPath = "Megumin.Binding.ICostomTestInterface/MyTestInnerClassField/MyTestInnerClassDeep2/MystringField1" };

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
                BindingPath = "Megumin.Binding.ICostomTestInterface/TypeProperty1",
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
        public List<IData> InterfaceTest = new List<IData>()
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
        public void Parse()
        {
            //var b = TestSO.BindInt;
            //b.ParseBinding(gameObject);
            //Debug.Log(b.Value);

            //GString222.ParseBinding(gameObject);
            //Debug.Log(GString222.Value);

            //Debug.Log(Time.fixedDeltaTime);

            var f = MyTestInnerClass;
            f.ParseBinding(gameObject, true);
            Debug.Log($"{f.BindingPath}   {f.Value}");
        }

        [Editor]
        public void SetValue()
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

#if UNITY_2023_1_OR_NEWER

        [Header("UNITY_2023_1_OR_NEWER  SerializeReference 泛型特化支持")]
        [SerializeReference]
        public IData mydata1 = new BindableValueInt();

        [SerializeReference]
        public IData<int> mydata2 = new BindableValueInt();

        [SerializeReference]
        public IData<int> mydata3 = new BindableValue<int>();

        [SerializeReference]
        public IData mydata4 = new BindableValue<int>();

        [SerializeReference]
        public List<IData> DatasList1 = new List<IData>()
        {
            new BindableValueInt(){ Value = 101},
            new BindableValue<int>{ Value = 102},
            new BindableValue<string>{Value = "MydataList_102"}
        };

        [SerializeReference]
        public List<IData<int>> DatasList2 = new List<IData<int>>()
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
            GUILayout.Label($"Value  :  {debugString}", GUILayout.ExpandWidth(true));

            var fields = this.GetType().GetFields();

            foreach (var field in fields)
            {
                if (typeof(IBindingParseable).IsAssignableFrom(field.FieldType))
                {
                    var p = (IBindingParseable)field.GetValue(this);
                    if (GUILayout.Button(field.Name))
                    {
                        p.ParseBinding(gameObject, true);
                        debugString = p.DebugParseResult();
                    }
                }
            }
            
            var properties = this.GetType().GetProperties();
            
            foreach (var property in properties)
            {
                if (typeof(IBindingParseable).IsAssignableFrom(property.PropertyType))
                {
                    var p = (IBindingParseable)property.GetValue(this);
                    if (GUILayout.Button(property.Name))
                    {
                        p.ParseBinding(gameObject, true);
                        debugString = p.DebugParseResult();
                    }
                }
            }

            GUILayout.EndArea();
        }
    }
}




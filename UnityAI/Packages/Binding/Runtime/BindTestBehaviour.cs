using System;
using System.Collections;
using System.Collections.Generic;
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

        public BindableValue<string> GameObjectTag
            = new BindableValue<string>() { BindingPath = "UnityEngine.GameObject/tag" };

        /// <summary>
        /// 多级字段绑定
        /// </summary>
        public BindableValue<string> GameObjectTransformTag
            = new BindableValue<string>() { BindingPath = "UnityEngine.GameObject/transform/tag" };

        /// <summary>
        /// 静态类型绑定
        /// </summary>
        public BindableValue<string> ApplicationVersion
            = new BindableValue<string>() { BindingPath = "UnityEngine.Application/version" };

        /// <summary>
        /// 静态类型绑定
        /// </summary>
        public BindableValue<float> TimeFixedDeltaTime
            = new BindableValue<float>() { BindingPath = "UnityEngine.Time/fixedDeltaTime" };

        /// <summary>
        /// 绑定非序列化类型
        /// </summary>
        public BindableValue<DateTimeOffset> DateTimeOffsetOffset
            = new BindableValue<DateTimeOffset>()
            { 
                DefaultValue = new DateTimeOffset(2000, 1, 1, 0, 0, 0, default) ,
                BindingPath = "System.DateTimeOffset/LocalDateTime",
            };

        /// <summary>
        /// 绑定非序列化类型
        /// </summary>
        public BindableValue<Type> BindType
            = new BindableValue<Type>()
            {
                DefaultValue = typeof(System.Version),
                BindingPath = "System.DateTimeOffset",
            };

        /// <summary>
        /// 绑定方法（0个参数，或者1个参数的某些特殊方法）
        /// </summary>
        public BindableValue<string> Test1
            = new BindableValue<string>() { BindingPath = "UnityEngine.GameObject/ToString()" };

        /// <summary>
        /// 绑定泛型方法
        /// </summary>
        public BindableValue<string> Test2
            = new BindableValue<string>() { BindingPath = "UnityEngine.Application/version" };

        /// <summary>
        /// 绑定扩展方法
        /// </summary>
        public BindableValue<string> Test3
           = new BindableValue<string>() { BindingPath = "UnityEngine.Application/version" };

        [SerializeReference]
        public List<BindableValueInt> IBindables = new List<BindableValueInt>();

        [SerializeReference]
        public List<IData> InterfaceTest = new List<IData>()
        {
            new BindableValueInt() { BindingPath = "UnityEngine.GameObject/layer" },
        };

        [Button]
        public void AddMiss()
        {
            IBindables.Clear();
            IBindables.Add(new BindableValueInt() { Key = nameof(TestSO.NeedOverrideInt1) });
            IBindables.Add(new BindableValueInt() { Key = nameof(TestSO.NeedOverrideInt2) });
            IBindables.Add(new BindableValueInt() { Key = nameof(TestSO.NeedOverrideInt3) });
        }

        [Button]
        public void Parse()
        {
            var b = TestSO.BindInt;
            b.InitializeBinding(gameObject);
            Debug.Log(b.Value);

            //GString222.InitializeBinding(gameObject);
            //Debug.Log(GString222.Value);

            //Debug.Log(Time.fixedDeltaTime);

            var f = ApplicationVersion;
            f.InitializeBinding(gameObject);
            Debug.Log(f.Value);
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
    }
}




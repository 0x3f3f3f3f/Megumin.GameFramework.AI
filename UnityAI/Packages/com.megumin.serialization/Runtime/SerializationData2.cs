using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Megumin.Serialization
{
    /// <summary>
    /// SerializationData解决不了同时支持List<Ref<>> 和Ref<List<>>问题。会导致循环嵌套。需要重新设计。
    /// </summary>
    [Serializable]
    public class SerializationData2
    {

        public class Basic
        {
            public string Data;
            public UnityEngine.Object refObject;
            /// <summary>
            /// 必须保存类型名，用于处理多态？ 
            /// 简单类型不存在基础关系？直接根据成员类型获取？
            /// </summary>
            public string T;
        }

        public string Data;
        public List<UnityEngine.Object> refObject = new();

        public bool TrySerialize(string name, object value)
        {

            return false;
        }
    }


}

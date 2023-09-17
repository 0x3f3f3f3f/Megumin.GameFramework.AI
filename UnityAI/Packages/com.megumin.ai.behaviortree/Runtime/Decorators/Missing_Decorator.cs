using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Megumin.Reflection;
using Megumin.Serialization;

namespace Megumin.AI.BehaviorTree
{
    /// <summary>
    /// 用于反序列化失败
    /// </summary>
    [Category("Debug")]
    [DisplayName("Missing")]
    [SerializationAlias("Megumin.AI.BehaviorTree.MissingDecorator")]
    public class Missing_Decorator : BTDecorator, IDetailable
    {
        public string MissType { get; set; }
        public ObjectData OrignalData { get; set; }

        public string GetDetail()
        {
            return MissType;
        }
    }
}



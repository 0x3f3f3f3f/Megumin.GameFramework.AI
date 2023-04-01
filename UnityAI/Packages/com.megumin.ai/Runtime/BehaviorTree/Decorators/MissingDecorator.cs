using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Megumin.Serialization;

namespace Megumin.GameFramework.AI.BehaviorTree
{
    /// <summary>
    /// 用于反序列化失败
    /// </summary>
    [Category("Debug")]
    public class MissingDecorator : BTDecorator, IDetailable
    {
        public string MissType { get; set; }
        public ObjectData OrignalData { get; set; }

        public string GetDetail()
        {
            return MissType;
        }
    }
}



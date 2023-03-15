using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Megumin.Binding
{
    /// <summary>
    /// 可以自动类型转换的
    /// </summary>
    public interface IAutoConvertable
    {

    }

    /// <summary>
    /// 可以自动转型的参数
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class AutoVar<T> : IAutoConvertable
    {
        /// <summary>
        /// 必然不是T类型，否则就不用转型了。
        /// </summary>
        public IRefable RefVar { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Megumin.GameFramework.AI
{
    public interface ITreeElement
    {
        /// <summary>
        /// 节点唯一ID
        /// </summary>
        string GUID { get; }
    }

    public class TreeElement : ITreeElement
    {
        /// <summary>
        /// 节点唯一ID
        /// </summary>
        [field: SerializeField]
        public string GUID { get; set; }

        public AITree Tree { get; set; }

        public virtual void Log(object message)
        {
            Tree?.Log(message);
        }
    }
}

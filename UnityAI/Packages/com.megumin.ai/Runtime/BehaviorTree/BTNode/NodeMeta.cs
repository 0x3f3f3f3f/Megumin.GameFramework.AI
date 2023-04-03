using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Megumin.GameFramework.AI.BehaviorTree
{
    [Serializable]
    public class NodeMeta: IAIMeta
    {
        public float x = 100;
        public float y = 100;
        /// <summary>
        /// TODO, index 要不要序列化，每次修改树节点会导致序列化文件大量变更。
        /// </summary>
        //public int index = -1;
        //public int depth = -1;
        public string Name;
        public string FriendlyName;
        public string Description;
        public string FriendlyDescription;
        public string Comment;
        public bool IsStartNode;

        public NodeMeta Clone()
        {
            return this.MemberwiseClone() as NodeMeta;
        }
    }
}

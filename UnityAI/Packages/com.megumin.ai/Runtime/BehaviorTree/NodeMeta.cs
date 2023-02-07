using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Megumin.GameFramework.AI.BehaviorTree
{
    [Serializable]
    public class NodeMeta
    {
        public float x;
        public float y;
        public int index;
        public string Name;
        public string FriendlyName;
        public string Description;
        public string FriendlyDescription;
        public string Comment;
    }
}

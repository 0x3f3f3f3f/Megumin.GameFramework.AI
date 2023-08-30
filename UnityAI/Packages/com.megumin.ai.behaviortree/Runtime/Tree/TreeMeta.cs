using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Megumin.AI.BehaviorTree
{
    [Serializable]
    public class TreeMeta : IAIMeta
    {
        public TreeMeta Clone()
        {
            return this.MemberwiseClone() as TreeMeta;
        }
    }
}

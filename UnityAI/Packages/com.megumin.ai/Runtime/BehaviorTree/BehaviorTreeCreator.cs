using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Megumin.Serialization;

namespace Megumin.GameFramework.AI.BehaviorTree
{
    public class BehaviorTreeCreator
    {
        public virtual BehaviorTree Instantiate(InitOption initOption, IRefFinder refFinder = null)
        {
            throw new NotImplementedException();
        }

        static Dictionary<string, BehaviorTreeCreator> cacheCreator = new();
        public static BehaviorTreeCreator GetCreator(string treeName, string guid, string version = null)
        {
            if (cacheCreator.TryGetValue(treeName, out var cr))
            {
                return cr;
            }
            else
            {
                //BehaviorTreeCreator creator = new BehaviorTreeCreator();
                //cacheCreator[treeName] = creator;
                //return creator;
                return null;
            }
        }
    }
}

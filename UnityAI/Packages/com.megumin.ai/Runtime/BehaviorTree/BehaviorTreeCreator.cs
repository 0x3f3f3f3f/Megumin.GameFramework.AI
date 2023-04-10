using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Megumin.Reflection;
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
            var creatorTypeName = $"Megumin.GameFramework.AI.BehaviorTree.{GetCreatorTypeName(treeName, guid)}";
            if (cacheCreator.TryGetValue(treeName, out var cr))
            {
                return cr;
            }
            else
            {
                if (TypeCache.TryGetType(creatorTypeName, out var type))
                {
                    var creator = Activator.CreateInstance(type) as BehaviorTreeCreator;
                    cacheCreator[treeName] = creator;
                    return creator;
                }

                return null;
            }
        }

        public static string GetCreatorTypeName(string treeName, string guid)
        {
            var creatorTypeName = $"BT_{treeName}_{guid}_Creator";
            creatorTypeName = creatorTypeName.Replace('-', '_');
            return creatorTypeName;
        }
    }
}

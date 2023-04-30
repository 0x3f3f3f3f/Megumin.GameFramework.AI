using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Megumin.Reflection;
using Megumin.Serialization;
using UnityEngine;

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
            StringBuilder sb = new StringBuilder();
            sb.Append("BT_");
            sb.Append(treeName);
            sb.Append("_");
            sb.Append(guid);
            sb.Append("_Creator");
            return sb.ToIdentifier();
        }

        public void PostInit(InitOption initOption, BehaviorTree tree)
        {
            if (initOption.LazyInitSubtree)
            {

            }
            else
            {
                foreach (var item in tree.AllNodes)
                {
                    if (item is SubTree subtreeNode)
                    {
                        subtreeNode.BehaviourTree
                            = tree.InstantiateSubTree(subtreeNode.BehaviorTreeAsset, subtreeNode);
                    }
                }
            }

            //回调
            foreach (var node in tree.AllNodes)
            {
                if (node is ISerializationCallbackReceiver nodeCallback)
                {
                    nodeCallback.OnAfterDeserialize();
                }

                foreach (var decorator in node.Decorators)
                {
                    if (decorator is ISerializationCallbackReceiver decoratorCallback)
                    {
                        decoratorCallback.OnAfterDeserialize();
                    }
                }
            }
        }
    }
}

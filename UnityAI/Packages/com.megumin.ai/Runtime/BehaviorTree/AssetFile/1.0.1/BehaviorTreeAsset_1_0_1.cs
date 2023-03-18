using System;
using System.Collections;
using System.Collections.Generic;
using Megumin.Serialization;
using UnityEngine;

namespace Megumin.GameFramework.AI.BehaviorTree
{
    public partial class BehaviorTreeAsset_1_0_1 : ScriptableObject, IBehaviorTreeAsset
    {
        public List<UnityObjectData> UnityObjectRef;
        public List<ObjectData> datas;

        public bool SaveTree(BehaviorTree tree)
        {
            if (tree == null)
            {
                return false;
            }

            if (!Guid.TryParse(tree.GUID, out var _))
            {
                tree.GUID = Guid.NewGuid().ToString();
            }

            Dictionary<object, string> cahce = new();
            Stack<(string name, object value)> needS = new();
            List<UnityObjectData> objRefs = new();
            cahce.Add(tree, tree.GUID);

            foreach (var variable in tree.Variable.Table)
            {
                cahce.Add(variable, variable.RefName);
                needS.Push((variable.RefName, variable));
            }

            foreach (var node in tree.AllNodes)
            {
                cahce.Add(node, node.GUID);
                needS.Push((node.GUID, node));

                foreach (var decorator in node.Decorators)
                {
                    cahce.Add(decorator, decorator.GUID);
                    needS.Push((decorator.GUID, decorator));
                }
            }

            List<ObjectData> AllSedRefData = new();
            while (needS.Count > 0)
            {
                if (AllSedRefData.Count > 10)
                {
                    Debug.LogError($"Too Large!!");
                    break;
                }

                var item = needS.Pop();
                ObjectData data = new ObjectData();
                if (data.TrySerialize(item.name, item.value, needS, objRefs, cahce))
                {
                    AllSedRefData.Add(data);
                }
            }

            AllSedRefData.Sort();
            datas = AllSedRefData;
            UnityObjectRef = objRefs;
            return true;
        }



        public UnityEngine.Object AssetObject => this;

        public BehaviorTree Instantiate(bool instanceMeta = true)
        {
            return new BehaviorTree();
        }
    }
}

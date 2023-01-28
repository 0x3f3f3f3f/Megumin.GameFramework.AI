using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Megumin.GameFramework.AI
{
    public class BehaviorTreeAsset : ScriptableObject
    {
        public string test = "aaa";
        public List<BehaviorTreeNodeAsset> Nodes = new List<BehaviorTreeNodeAsset>();
        public BehaviorTreeNodeAsset StartNode;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Megumin.GameFramework.AI
{
    public class BehaviorTreeNodeAsset : ScriptableObject
    {
        public string Guid;
        public Vector2 Position;
        public string Type;
        public string Name;
        public string Description;

        public List<BehaviorTreeNodeAsset> Children = new List<BehaviorTreeNodeAsset>();
        /// <summary>
        /// 允许多个父
        /// </summary>
        public List<BehaviorTreeNodeAsset> Parents = new List<BehaviorTreeNodeAsset>();
    }
}

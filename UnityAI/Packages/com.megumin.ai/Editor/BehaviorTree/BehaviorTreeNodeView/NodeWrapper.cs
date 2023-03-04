using System.ComponentModel;
using UnityEngine;

namespace Megumin.GameFramework.AI.BehaviorTree.Editor
{
    public class NodeWrapper : ScriptableObject
    {
        [SerializeReference]
        public BTNode Node;

        public BehaviorTreeNodeView View { get; internal set; }

        [Editor]
        public void Test()
        {
            if (View.outputContainer.ClassListContains("unDisplay"))
            {
                View.outputContainer.RemoveFromClassList("unDisplay");
            }
            else
            {
                View.outputContainer.AddToClassList("unDisplay");
            }
        }
    }
}

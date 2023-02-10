using System.Collections;
using System.ComponentModel;
using UnityEngine;

namespace Megumin.GameFramework.AI.BehaviorTree
{
    public class BehaviorTreeRunner : MonoBehaviour
    {
        BehaviorTree BehaviourTree;
        public BehaviorTreeAsset BehaviorTreeAsset;
        private void Awake()
        {
            BehaviourTree = BehaviorTreeAsset.Instantiate();
            BehaviourTree.Init(gameObject);
        }

        private void OnEnable()
        {
            
        }

        // Use this for initialization
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {
            BehaviourTree.Tick();
        }

        [Editor]
        public void ResetTree()
        {
            BehaviourTree.Reset();
        }
    }
}
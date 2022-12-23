using System.Collections;
using UnityEngine;

namespace Megumin.GameFramework.AI.BehaviorTree
{
    public class BehaviorTreeRunner : MonoBehaviour
    {
        BehaviorTree BehaviourTree = new MyTestBehaviourTree();
        private void Awake()
        {
            BehaviourTree.Load();
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
    }
}
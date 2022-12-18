using System.Collections;
using UnityEngine;

namespace Megumin.GameFramework.AI.BehaviorTree
{
    public class BehaviourTreeRunner : MonoBehaviour
    {
        BehaviourTree BehaviourTree = new MyTestBehaviourTree();
        private void Awake()
        {
            BehaviourTree.Load();
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
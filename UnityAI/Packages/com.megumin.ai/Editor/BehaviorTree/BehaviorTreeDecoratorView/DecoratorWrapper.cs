using System.ComponentModel;
using UnityEngine;

namespace Megumin.GameFramework.AI.BehaviorTree.Editor
{


    public class DecoratorWrapper : ScriptableObject
    {
        [SerializeReference]
        public ITreeElement Decorator;



        [Editor]
        public void Test()
        {

        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Megumin.GameFramework.AI.BehaviorTree.Editor
{
    public class TreeWrapper : ScriptableObject, ISerializationCallbackReceiver
    {
        public BehaviorTree Tree;
        public int ChangeVersion;

        public void OnBeforeSerialize()
        {

        }

        public void OnAfterDeserialize()
        {
            //Debug.Log("OnAfterDeserialize--------------------");
            //Undo/Redo 会导致StartNode不正确。重写赋值一次
            if (Tree != null)
            {
                Tree.ReCacheDic();
            }
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Megumin.Binding;
using UnityEditor;
using UnityEngine;

namespace Megumin.AI.BehaviorTree
{
    [Serializable]
    public class MyTestVarData
    {
        public int a = 0;
    }

    [Serializable]
    [DebuggerTypeProxy(typeof(DebugView))]
    public class RefVar_MyTestVarData : RefVar<MyTestVarData>
    {

    }

    public class VariableCreator_MyTestVarData : VariableCreator
    {
        public override string Name { get; set; } = "MyTest/Date";

        public override IRefable Create()
        {
            return new RefVar_MyTestVarData() { RefName = "MyTestVarData" };
        }


#if UNITY_EDITOR
        [UnityEditor.InitializeOnLoadMethod]
#endif
        //[UnityEngine.RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void AddToAllCreator()
        {
            VariableCreator.AllCreator.Add(new VariableCreator_MyTestVarData());
            //Or insert.
            //VariableCreator.AllCreator.Insert(0,new VariableCreator_MyTestVarData());
        }
    }
}






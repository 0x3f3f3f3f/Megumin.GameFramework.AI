using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using System;
using UnityEditor;
using Megumin.GameFramework.AI.Editor;
using System.Linq;

namespace Megumin.GameFramework.AI.BehaviorTree.Editor
{
    public partial class BehaviorTreeView
    {


        private string OnSerializeGraphElements(IEnumerable<GraphElement> elements)
        {
            this.LogFuncName();
            return default;
        }

        private bool OnCanPasteSerializedData(string data)
        {
            this.LogFuncName();
            return default;
        }

        private void OnUnserializeAndPaste(string operationName, string data)
        {
            this.LogFuncName();
        }
    }
}

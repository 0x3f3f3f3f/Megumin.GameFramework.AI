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
        internal HashSetScope UndoMute = new();
        public void UndoRecord(string name)
        {
            if (UndoMute)
            {
                Debug.Log($"UndoRecord 被禁用或合并。User:  [{UndoMute.LogUsers}    ]   RecordName:  {name}");
            }
            else
            {
                CreateTreeSOTreeIfNull();
                Undo.RecordObject(SOTree, name);
                SOTree.ChangeVersion++;
                LoadVersion = SOTree.ChangeVersion;

                EditorWindow.UpdateHasUnsavedChanges();
            }
        }

        public IDisposable UndoBeginScope(string name)
        {
            UndoRecord(name);
            return UndoMute.Enter(name);
        }
    }
}

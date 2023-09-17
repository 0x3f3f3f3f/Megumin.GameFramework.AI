using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using System;
using UnityEditor;
using Megumin.AI.Editor;
using System.Linq;
using System.Threading;

namespace Megumin.AI.BehaviorTree.Editor
{
    public partial class BehaviorTreeView
    {
        internal HashSetScope UndoMute = new();

        /// <summary>
        /// 记录一个版本，方便Undo回来
        /// </summary>
        /// <param name="name"></param>
        public void UndoRecord(string name)
        {
            if (UndoMute)
            {
                if (BehaviorTreeEditor.EditorLog)
                {
                    Debug.Log($"UndoRecord Muted。User:  [{UndoMute.LogUsers}    ]   RecordName:  {name}");
                    //Debug.Log($"UndoRecord 被禁用或合并。User:  [{UndoMute.LogUsers}    ]   RecordName:  {name}");
                }
            }
            else
            {
                CreateTreeSOTreeIfNull();

                //this.LogMethodName(name);

                Undo.RecordObject(SOTree, name);
                SOTree.ChangeVersion++;
                LoadVersion = SOTree.ChangeVersion;

                EditorWindow?.UpdateHasUnsavedChanges();
            }
        }

        /// <summary>
        /// 增加ChangeVersion，用于Inspector和外部修改
        /// </summary>
        public void IncrementChangeVersion(string name)
        {
            CreateTreeSOTreeIfNull();

            //this.LogMethodName(name);

            //Undo.RecordObject(SOTree, name);
            SOTree.ChangeVersion++;
            //LoadVersion = SOTree.ChangeVersion;

            EditorWindow?.UpdateHasUnsavedChanges();
        }

        /// <summary>
        /// 开启一个保存版本的作用域，在释放前的所有操作，都认为是同一个操作。
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public IDisposable UndoBeginScope(string name)
        {
            UndoRecord(name);
            return UndoMute.Enter(name);
        }
    }
}

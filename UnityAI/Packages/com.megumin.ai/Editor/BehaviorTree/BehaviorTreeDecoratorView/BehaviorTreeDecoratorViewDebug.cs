using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Megumin.GameFramework.AI.Editor;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Megumin.GameFramework.AI.BehaviorTree.Editor
{
    public partial class BehaviorTreeDecoratorView
    {
        bool isRunning = false;
        Status lastTickState = Status.Init;

        internal void OnPostTick()
        {
            if (Decorator == null || Decorator.Owner == null)
            {
                return;
            }

            isRunning = Decorator.Owner.State == Status.Running;

            if (isRunning)
            {
                RefreshDetail();
            }



            if (lastTickState != Decorator.Owner.State)
            {
                RefreshDetail();
            }

            lastTickState = Decorator.Owner.State;
        }
    }
}

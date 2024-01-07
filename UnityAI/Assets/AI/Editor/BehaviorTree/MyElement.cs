using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Megumin.GameFramework.AI.Editor
{
    public class MyElement : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<MyElement> 
        {
            public override VisualElement Create(IUxmlAttributes bag, CreationContext cc)
            {
                return base.Create(bag, cc);
            }
        }

        public MyElement()
        {
            var lable = new Label();
            lable.name = "TestLabel";
            this.Add(lable);    
        }

    }
}

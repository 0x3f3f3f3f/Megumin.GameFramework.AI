using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Megumin.AI.BehaviorTree.Editor
{
    public class CreateNewNodeScript
    {
        public static void CreateNewTemplateScript(string name)
        {
            ProjectWindowUtil.CreateScriptAssetFromTemplateFile(GetTemplatePath(name), name.Replace("Template", ""));
        }

        public static string GetTemplatePath(string name)
        {
            var template = Resources.Load<TextAsset>(name);
            var path = AssetDatabase.GetAssetPath(template);
            return path;
        }

        [MenuItem("Assets/Create/Megumin/AI/ActionNode C# Script", false)]
        public static void CreateNewActionNode()
        {
            CreateNewTemplateScript("NewActionNodeTemplate.cs");
        }

        [MenuItem("Assets/Create/Megumin/AI/Decorator C# Script", false)]
        public static void CreateNewDecorator()
        {
            CreateNewTemplateScript("NewDecoratorTemplate.cs");
        }

        [MenuItem("Assets/Create/Megumin/AI/ConditionDecorator C# Script", false)]
        public static void CreateNewConditionDecorator()
        {
            CreateNewTemplateScript("NewConditionDecoratorTemplate.cs");
        }

        [MenuItem("Assets/Create/Megumin/AI/Variable C# Script", false)]
        public static void CreateNewVariable()
        {
            CreateNewTemplateScript("NewVariableTemplate.cs");
        }
    }
}





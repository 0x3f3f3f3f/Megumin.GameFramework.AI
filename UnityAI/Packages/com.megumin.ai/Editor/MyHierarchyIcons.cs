using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using Megumin.GameFramework.AI.BehaviorTree;

namespace Megumin.GameFramework.AI.Editor
{

    /// <summary>
    /// https://answers.unity.com/questions/431952/how-to-show-an-icon-in-hierarchy-view.html
    /// </summary>
    [InitializeOnLoad]
    class MyHierarchyIcons
    {
        static readonly Texture2D Icon;

        static MyHierarchyIcons()
        {
            var path = AssetDatabase.GUIDToAssetPath("97672047d59919945853bfeb83fdc913");
            var imp = AssetImporter.GetAtPath(path);

            if (imp is MonoImporter mono)
            {
                Icon = mono.GetIcon();
            }
            else if (imp is PluginImporter plugin)
            {
                Icon = plugin.GetIcon(nameof(BehaviorTreeRunner));
            }

            EditorApplication.hierarchyWindowItemOnGUI -= HierarchyItemCB;
            EditorApplication.hierarchyWindowItemOnGUI += HierarchyItemCB;
            //EditorApplication.hierarchyChanged
        }

        static void HierarchyItemCB(int instanceID, Rect selectionRect)
        {
            selectionRect.xMin = selectionRect.xMax - 18;

            GameObject go = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
            if (go == null)
            {
                return;
            }

            if (go.GetComponent<BehaviorTreeRunner>())
            {
                GUI.Label(selectionRect, Icon);
            }
        }

        public static Texture2D GetImporterIcon(MonoBehaviour monoBehaviour)
        {
            var script = MonoScript.FromMonoBehaviour(monoBehaviour);
            var path = AssetDatabase.GetAssetPath(script);

            var imp = AssetImporter.GetAtPath(path);

            if (imp is MonoImporter mono)
            {
                return mono.GetIcon();
            }
            else if (imp is PluginImporter plugin)
            {
                return plugin.GetIcon(monoBehaviour.GetType().Name);
            }

            return null;
        }
    }
}





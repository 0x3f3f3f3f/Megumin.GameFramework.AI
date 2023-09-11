using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Megumin.Reflection.Editor
{
    internal class Test
    {
        [MenuItem("Tools/Megumin/Reflection/TypeCache Test")]
        public static void TestButton()
        {
            Megumin.Reflection.TypeCache.Test();
        }

        [MenuItem("Tools/Megumin/Reflection/TypeCache Clear")]
        public static void Clear()
        {
            Megumin.Reflection.TypeCache.Clear();
        }

        [MenuItem("Tools/Megumin/Reflection/TypeCache ClearHotType")]
        public static void ClearHotType()
        {
            Megumin.Reflection.TypeCache.ClearHotType();
        }

        [MenuItem("Tools/Megumin/Reflection/TypeCache HotAllTypeAlias")]
        public static void HotAllTypeAlias()
        {
            Megumin.Reflection.TypeCache.HotAllTypeAlias();
        }
    }
}

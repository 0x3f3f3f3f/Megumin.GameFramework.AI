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
    }
}

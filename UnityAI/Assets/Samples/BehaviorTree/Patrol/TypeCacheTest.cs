using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Megumin
{
    public class TypeCacheTest : MonoBehaviour
    {
        public static void CacheAllTypes()
        {
            Megumin.Reflection.TypeCache.CacheAllTypes();
        }

        public static async void CacheAllTypesAsync()
        {
            await Megumin.Reflection.TypeCache.CacheAllTypesAsync();
            Debug.Log("CacheAllTypesAsync End");
        }

        public static void Clear()
        {
            Megumin.Reflection.TypeCache.Clear();
        }
    }
}



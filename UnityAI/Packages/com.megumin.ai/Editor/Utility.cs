﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

namespace Megumin.GameFramework.AI.Editor
{
    public static class Utility
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T CreateSOWrapper<T>()
            where T : ScriptableObject
        {
            var wrapper = ScriptableObject.CreateInstance<T>();
            //内存中的SO对象改动，会让Scene变成dirty状态，使用 HideFlags.DontSave，就不会影响 Scene状态。
            wrapper.hideFlags = HideFlags.DontSave;
            return wrapper;
        }
    }

    internal static class Extension
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T CreateSOWrapper<T>(this IEventHandler @object)
            where T : ScriptableObject
        {
            return Utility.CreateSOWrapper<T>();
        }
    }
}

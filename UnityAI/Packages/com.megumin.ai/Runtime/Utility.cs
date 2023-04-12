using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UIElements;
using Debug = UnityEngine.Debug;

[assembly: InternalsVisibleTo("Megumin.GameFramework.AI.Editor")]
namespace Megumin.GameFramework.AI
{
    public static class Utility
    {

    }

    internal static class Extension
    {
        [HideInCallstack]
        [DebuggerHidden]
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void LogMethodName(this UnityEngine.Object @object,
                                         object state = null,
                                         object state1 = null,
                                         object state2 = null,
                                         object state3 = null,
                                         [CallerMemberName] string funcName = null)
        {
            if (state == null)
            {
                Debug.Log(funcName);
            }
            else
            {
                Debug.Log($"{funcName}    {state}    {state1}    {state2}    {state3}");
            }
        }

        [HideInCallstack]
        [DebuggerHidden]
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void LogMethodName(this IEventHandler @object,
                                         object state = null,
                                         object state1 = null,
                                         object state2 = null,
                                         object state3 = null,
                                         [CallerMemberName] string funcName = null)
        {
            if (state == null)
            {
                Debug.Log(funcName);
            }
            else
            {
                Debug.Log($"{funcName}    {state}    {state1}    {state2}    {state3}");
            }
        }

        [HideInCallstack]
        [DebuggerHidden]
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void LogMethodName(this IManipulator @object,
                                         object state = null,
                                         object state1 = null,
                                         object state2 = null,
                                         object state3 = null,
                                         [CallerMemberName] string funcName = null)
        {
            if (state == null)
            {
                Debug.Log(funcName);
            }
            else
            {
                Debug.Log($"{funcName}    {state}    {state1}    {state2}    {state3}");
            }
        }

        public static bool TryGetAttribute<T>(this Type type, out T attribute)
             where T : Attribute
        {
            var attri = type?.GetCustomAttribute<T>();
            if (attri != null)
            {
                attribute = attri;
                return true;
            }
            attribute = null;
            return false;
        }


    }
}

#if UNITY_2022_2_OR_NEWER
    //Unity 内置
#else

namespace UnityEngine
{
    //
    // 摘要:
    //     Marks the methods you want to hide from the Console window callstack. When you
    //     hide these methods they are removed from the detail area of the selected message
    //     in the Console window.
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class HideInCallstackAttribute : Attribute
    {
    }
}

#endif


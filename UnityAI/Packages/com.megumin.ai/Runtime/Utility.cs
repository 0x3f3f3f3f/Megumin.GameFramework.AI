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


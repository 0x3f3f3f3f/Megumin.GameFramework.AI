using System.Diagnostics;
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
    }
}

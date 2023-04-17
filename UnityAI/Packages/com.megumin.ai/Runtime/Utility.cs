using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;
using Debug = UnityEngine.Debug;

[assembly: InternalsVisibleTo("Megumin.GameFramework.AI.Editor")]
namespace Megumin.GameFramework.AI
{
    public static class Utility
    {

    }

    internal static class Extension_DF244A1F18D4424F8B4CB0DFC9EFC825
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="visualElement"></param>
        /// <param name="className"></param>
        /// <param name="enable"></param>
        /// <returns>ClassList 是否发生变化</returns>
        public static bool SetToClassList(this VisualElement visualElement, string className, bool enable = true)
        {
            bool hasClass = visualElement.ClassListContains(className);
            if (enable)
            {
                if (hasClass)
                {
                    return false;
                }
                else
                {
                    visualElement.AddToClassList(className);
                    return true;
                }
            }
            else
            {
                if (hasClass)
                {
                    visualElement.RemoveFromClassList(className);
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public static Task Delay(this VisualElement visualElement, long delayMs)
        {
            TaskCompletionSource<bool> taskCompletionSource = new();
            visualElement.schedule.Execute(() => { taskCompletionSource.TrySetResult(true); }).ExecuteLater(delayMs);
            return taskCompletionSource.Task;
        }

        public static string GetTitle<T>(this T obj)
        {
            var title = "Unknown";

            if (obj is ITitleable titleable)
            {
                title = titleable.Title;
            }
            else
            {
                var type = obj?.GetType();
                if (type != null)
                {
                    title = type.Name;
                    var attribute = type.GetCustomAttribute<DisplayNameAttribute>(false);
                    if (attribute != null)
                    {
                        title = attribute.DisplayName;
                    }
                }
            }

            return title;
        }


        public static bool TryGetIconPath(this Type type, out string path)
        {
            if (type != null && Attribute.IsDefined(type, typeof(IconAttribute)))
            {
                var attributes = type.GetCustomAttributes(typeof(IconAttribute), true);
                for (int i = 0, c = attributes.Length; i < c; i++)
                {
                    if (attributes[i] is IconAttribute)
                    {
                        path = ((IconAttribute)attributes[i]).path;
                        return true;
                    }
                }
            }

            path = null;
            return false;
        }
    }
}


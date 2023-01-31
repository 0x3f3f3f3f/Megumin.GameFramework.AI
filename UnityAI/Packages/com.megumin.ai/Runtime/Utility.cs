using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UIElements;

[assembly: InternalsVisibleTo("Megumin.GameFramework.AI.Editor")]
namespace Megumin.GameFramework.AI
{
    public static class Utility
    {

    }
    internal static class Extension
    {
        public static void LogFuncName(this UnityEngine.Object @object, object state = null, [CallerMemberName] string funcName = null)
        {
            if (state == null)
            {
                Debug.Log(funcName);
            }
            else
            {
                Debug.Log($"{funcName}  {state}");
            }
        }

        public static void LogFuncName(this IEventHandler @object, object state = null, [CallerMemberName] string funcName = null)
        {
            if (state == null)
            {
                Debug.Log(funcName);
            }
            else
            {
                Debug.Log($"{funcName}  {state}");
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

namespace Megumin.GameFramework.AI
{
    internal static class Utility
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

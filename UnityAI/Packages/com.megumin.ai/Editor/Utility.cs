using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Megumin.GameFramework.AI
{
    internal static class Utility
    {
        public static void LogFuncName(this UnityEngine.Object @object, [CallerMemberName] string funcName = null)
        {
            Debug.Log(funcName);
        }
    }
}

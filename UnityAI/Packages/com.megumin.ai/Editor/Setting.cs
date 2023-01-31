using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.SettingsManagement;

namespace Megumin.GameFramework.AI.Editor
{
    static class MySettingsManager
    {
        // Replace this with your own package name. Project settings will be stored in a JSON file in a directory matching
        // this name.
        internal const string k_PackageName = "com.megumin.ai";

        static Settings s_Instance;

        internal static Settings instance
        {
            get
            {
                if (s_Instance == null)
                    s_Instance = new Settings(k_PackageName);

                return s_Instance;
            }
        }
    }

    // Usually you will only have a single Settings instance, so it is convenient to define a UserSetting<T> implementation
    // that points to your instance. In this way you avoid having to pass the Settings parameter in setting field definitions.
    class MySetting<T> : UserSetting<T>
    {
        public MySetting(string key, T value, SettingsScope scope = SettingsScope.Project)
            : base(MySettingsManager.instance, key, value, scope)
        { }

        MySetting(Settings settings, string key, T value, SettingsScope scope = SettingsScope.Project)
            : base(settings, key, value, scope) { }
    }
}
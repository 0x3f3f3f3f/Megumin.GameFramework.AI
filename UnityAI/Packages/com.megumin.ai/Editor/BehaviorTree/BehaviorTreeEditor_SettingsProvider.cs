using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Megumin.GameFramework.AI.Editor;
using UnityEditor;
using UnityEditor.SettingsManagement;
using UnityEngine;

namespace Megumin.GameFramework.AI.BehaviorTree.Editor
{
    public partial class BehaviorTreeEditor
    {
        [SettingsProvider]
        static SettingsProvider CreateSettingsProvider()
        {
            var provider = new UserSettingsProvider("Preferences/Megumin/BehaviorTree",
                                                    MySettingsManager.instance,
                                                    new[] { typeof(BehaviorTreeEditor).Assembly });
            return provider;
        }

        [UserSetting("LogSetting", "EditorLog")]
        public static MySetting<bool> EditorLog = new("EditorLog", false, SettingsScope.User);
        //[UserSetting("LogSetting", "EditorLogLanguage")]
        //public static MySetting<SystemLanguage> EditorLogLanguage = 
        //    new("EditorLogLanguage", SystemLanguage.English, SettingsScope.User);
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.SettingsManagement;
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

        public static void RepaintWindows(string title = "Inspector")
        {
            var resultWindows = FindWindows<EditorWindow>(title);

            foreach (var item in resultWindows)
            {
                item.Repaint();
            }
        }

        public static IEnumerable<EditorWindow> FindWindowsByTypeName(string windowTypeName)
        {
            EditorWindow[] array = Resources.FindObjectsOfTypeAll(typeof(EditorWindow)) as EditorWindow[];
            var resultWindows = from wnd in array
                                where wnd.GetType().Name == windowTypeName
                                select wnd;
            return resultWindows;
        }

        public static IEnumerable<T> FindWindows<T>(string title = null)
            where T : EditorWindow
        {
            T[] array = Resources.FindObjectsOfTypeAll(typeof(T)) as T[];

            if (string.IsNullOrEmpty(title))
            {
                return array;
            }

            var resultWindows = from wnd in array
                                where wnd.titleContent.text == title
                                select wnd;
            return resultWindows;
        }

        public static ValueTask<MonoScript> GetMonoScript(Type type)
        {
            //Todo Cache /unity background tasks
            var scriptGUIDs = AssetDatabase.FindAssets($"t:script");

            foreach (var scriptGUID in scriptGUIDs)
            {
                var assetPath = AssetDatabase.GUIDToAssetPath(scriptGUID);
                var script = AssetDatabase.LoadAssetAtPath<MonoScript>(assetPath);
                var code = script.text;
                if (code.Contains($"class {type.Name}")
                    && code.Contains(type.Namespace))
                {
                    return new ValueTask<MonoScript>(script);
                }
            }

            return new ValueTask<MonoScript>(result: null);
        }

        public static async void OpenScript(Type type)
        {
            var obj = await GetMonoScript(type);
            if (obj)
            {
                AssetDatabase.OpenAsset(obj, 0, 0);
            }
        }

        public static async void SelectScript(Type type)
        {
            var obj = await Utility.GetMonoScript(type);
            if (obj)
            {
                Selection.activeObject = obj;
            }
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

        public static void RepaintInspectorWindows(this IEventHandler @object)
        {
            @object.LogMethodName();
            Utility.RepaintWindows("Inspector");
        }

        public static void SetToClassList(this VisualElement visualElement, string className, bool enable = true)
        {
            if (enable)
            {
                visualElement.AddToClassList(className);
            }
            else
            {
                visualElement.RemoveFromClassList(className);
            }
        }

        public static bool WasHitByMouse<T>(this VisualElement target, MouseEventBase<T> evt)
            where T : MouseEventBase<T>, new()
        {
            if (target != null
                && target.enabledInHierarchy
                && target.pickingMode != PickingMode.Ignore)
            {
                if (target is ISelectable selectable)
                {
                    if (selectable.IsSelectable())
                    {
                        Vector2 localMousePosition = target.WorldToLocal(evt.mousePosition);
                        if (selectable.HitTest(localMousePosition))
                        {
                            return true;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return true;
                }
            }

            return false;
        }

        public static void AppendAction(this DropdownMenu menu,
                                        UserSetting<bool> setting,
                                        string actionName = null,
                                        Action<UserSetting<bool>> action = null)
        {
            if (string.IsNullOrEmpty(actionName))
            {
                actionName = setting?.key;
            }

            menu.AppendAction(actionName,
                a =>
                {
                    setting.SetValue(!setting);
                    action?.Invoke(setting);
                },
                a =>
                {
                    if (setting == null)
                    {
                        return DropdownMenuAction.Status.Disabled;
                    }
                    return setting ? DropdownMenuAction.Status.Checked : DropdownMenuAction.Status.Normal;
                });
        }

        public static void TrySetIconFromAttribute(this VisualElement iconElement, Type type)
        {
            string path = GetIconPathFromAttribute(type);
            if (TryLoadIcon(path, out var icon))
            {
                iconElement.style.backgroundImage = icon;
            }
        }

        static Dictionary<string, Texture2D> iconCache = new();
        public static bool TryLoadIcon(string path, out Texture2D texture2D)
        {
            if (string.IsNullOrEmpty(path))
            {
                texture2D = default;
                return false;
            }

            if (!iconCache.TryGetValue(path, out texture2D))
            {
                //Resources.Load 需要路径不包含扩展名
                var withoutExtensionPath = path.Replace(Path.GetExtension(path), "");
                var iconTexture = Resources.Load<Texture2D>(withoutExtensionPath);

                if (!iconTexture)
                {
                    iconTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
                }

                if (!iconTexture)
                {
                    var loadIcon = typeof(EditorGUIUtility).GetMethod("LoadIcon", BindingFlags.Static | BindingFlags.NonPublic);
                    iconTexture = loadIcon.Invoke(null, new object[] { path }) as Texture2D;
                }

                iconCache[path] = iconTexture;
                texture2D = iconTexture;
            }

            return texture2D;
        }

        public static string GetIconPathFromAttribute(Type type)
        {
            if (Attribute.IsDefined(type, typeof(IconAttribute)))
            {
                var attributes = type.GetCustomAttributes(typeof(IconAttribute), true);
                for (int i = 0, c = attributes.Length; i < c; i++)
                {
                    if (attributes[i] is IconAttribute)
                    {
                        return ((IconAttribute)attributes[i]).path;
                    }
                }
            }
            return null;
        }
    }
}

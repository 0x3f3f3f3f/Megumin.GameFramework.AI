using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using System.Reflection;
using UnityEngine.UIElements;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.UIElements;
#endif

namespace Megumin.Binding
{
    public class BindingPathSetterAttribute : PropertyAttribute
    {
    }


#if UNITY_EDITOR

    [UnityEditor.CustomPropertyDrawer(typeof(BindingPathSetterAttribute))]
    public class BindingPathAttributeDrawer : UnityEditor.PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            //使用StyleSheet代替 C# styles override inline
            //https://docs.unity3d.com/Documentation/Manual/UIE-uss-selector-precedence.html
            //https://forum.unity.com/threads/uxml-parsing-at-runtime.1327311/
            //必须使用uss文件，没有办法从一个uss字符解析。
            StyleSheet styleSheet = Resources.Load<StyleSheet>("BindingPathSetterAttribute");
            var container = new VisualElement();
            container.AddToClassList("bindingPathSetterAttribute");
            container.name = "bindingPathSetterAttribute";

            container.styleSheets.Add(styleSheet);
            //container.style.flexDirection = FlexDirection.Row;

            var field = new PropertyField();
            //field.style.flexGrow = 1;
            field.AddToClassList("bindingPath");
            field.BindProperty(property);
            //field.RegisterCallback<ContextualMenuPopulateEvent>(evt =>
            //{
            //    evt.menu.AppendAction("Set BindingPath", 
            //        async a => 
            //        {
            //            var gType = fieldInfo.DeclaringType.GetGenericArguments()[0];
            //            var path = await BindingEditor.GetBindStr(gType,true);
            //            Debug.Log(path + "-----------------");
            //            property.stringValue = path;
            //        }, 
            //        DropdownMenuAction.Status.Normal);
            //});

            var settingButton = new Button();
            settingButton.AddToClassList("settingButton");
            //settingButton.style.width = settingButtonWidth;
            settingButton.tooltip = "Set BindingPath";
            //settingButton.style.backgroundImage = settingIcon.image as Texture2D;

            var gType = fieldInfo.DeclaringType.GetGenericArguments()[0];

            settingButton.clicked += async () =>
            {
                var path = await BindingEditor.GetBindStr(gType, true);

                //TODO, Editor hasunsavechange
                //Undo.RecordObject(property.serializedObject.targetObject, $"Set BindingPath {property.propertyPath}");
                //Debug.Log(path + "-----------------");
                //property.SetValue(path);

                //此时已经无法给property赋值了。改为给TextField赋值。
                var textField = field.Q<TextField>();
                textField.value = path;
            };

            container.Add(field);
            container.Add(settingButton);
            return container;
        }


        public override float GetPropertyHeight(UnityEditor.SerializedProperty property, GUIContent label)
        {
            return UnityEditor.EditorGUI.GetPropertyHeight(property, true);
        }

        static GUIContent settingIcon =
            new(EditorGUIUtility.IconContent("settings icon")) { tooltip = "Set BindingPath" };

        const int settingButtonWidth = 26;
        const int testButtonWidth = 36;

        Dictionary<string, ParseBindingResult> parseResult = new Dictionary<string, ParseBindingResult>();
        public override void OnGUI(Rect position, UnityEditor.SerializedProperty property, GUIContent label)
        {
            var propertyPosition = position;
            propertyPosition.width -= settingButtonWidth + testButtonWidth + 2;

            var buttonPosition = position;
            buttonPosition.width = settingButtonWidth;
            buttonPosition.x += position.width - settingButtonWidth;

            UnityEditor.EditorGUI.PropertyField(propertyPosition, property, label, true);
            var gType = fieldInfo.DeclaringType.GetGenericArguments()[0];
            if (property.GetBindintString(GUI.Button(buttonPosition, settingIcon), out var str, gType))
            {
                property.stringValue = str;
            }

            var buttonPositionTest = position;
            buttonPositionTest.width = testButtonWidth;
            buttonPositionTest.x += position.width - settingButtonWidth - testButtonWidth;

            if (GUI.Button(buttonPositionTest, $"Test"))
            {
                //通过property取得实例对象
                //https://gist.github.com/douduck08/6d3e323b538a741466de00c30aa4b61f

                var obj = property.serializedObject.targetObject;
                object data = null;
                if (property.propertyPath.EndsWith("]"))
                {
                    data = property.managedReferenceValue;
                }
                else
                {
                    data = this.fieldInfo.GetValue(obj);
                }

                if (data == null)
                {

                }

                if (data is IBindingParseable parseable)
                {
                    GameObject gameObject = obj as GameObject;
                    if (obj is Component component)
                    {
                        gameObject = component.gameObject;
                    }
                    parseResult[property.propertyPath]
                        = parseable.ParseBinding(gameObject, true);
                    parseable.DebugParseResult();
                }

                //fieldInfo = this.fieldInfo; 
                //var field2 = this.fieldInfo;
                //var v = field2.GetValue(property.serializedObject.targetObject);
                //var index = property.enumValueIndex;

                ////Debug.Log(property.serializedObject.targetObject);

                //Type type = obj.GetType();
                //var fieldInfo = type.GetField(property.propertyPath);
                //var fValue = fieldInfo.GetValue(obj);

                //if (fValue is IBindingParseable parseable)
                //{
                //    GameObject gameObject = obj as GameObject;
                //    if (obj is Component component)
                //    {
                //        gameObject = component.gameObject;
                //    }
                //    parseable.ParseBinding(gameObject, true);
                //    parseable.DebugParseResult();
                //}
            }
        }
    }

    public static class BindingEditor
    {
        public static bool GetBindintString(this SerializedProperty property,
                                            bool click,
                                            out string str,
                                            Type matchType = null,
                                            bool autoConvert = true)
        {
            if (click)
            {
                BindingEditor.GetBindStr(property.propertyPath, matchType, autoConvert);
            }

            if (BindingEditor.cacheResult.TryGetValue(property.propertyPath, out str))
            {
                BindingEditor.cacheResult.Remove(property.propertyPath);
                return true;
            }
            str = default;
            return false;
        }

        public static Dictionary<string, string> cacheResult = new Dictionary<string, string>();
        public static async void GetBindStr(string propertyPath)
        {
            var str = await BindingEditor.GetBindStr<int>();
            cacheResult[propertyPath] = str;
        }

        public static async void GetBindStr(string propertyPath, Type matchType, bool autoConvert = true)
        {
            var str = await BindingEditor.GetBindStr(matchType, autoConvert);
            cacheResult[propertyPath] = str;
        }


        public static Task<string> GetBindStr<T>()
        {
            return GetBindStr(typeof(T));
        }

        public static Task<string> GetBindStr(Type matchType, bool autoConvert = true)
        {
            TaskCompletionSource<string> source = new TaskCompletionSource<string>();
            GenericMenu.MenuFunction2 func = ms =>
            {
                if (ms is List<MemberInfo> members)
                {
                    string str = "";


                    for (int i = 0; i < members.Count; i++)
                    {
                        var item = members[i];
                        if (item is TypeInfo typeInfo)
                        {
                            str += $"{typeInfo.FullName}/";
                        }
                        else
                        {
                            if (i == members.Count - 1)
                            {
                                str += $"{item.Name}";
                            }
                            else
                            {
                                str += $"{item.Name}/";
                            }
                        }
                    }
                    source.SetResult(str);
                }
            };

            GenericMenu bindMenu = GetMenu(matchType, func, autoConvert);
            bindMenu.ShowAsContext();
            return source.Task;
        }

        public static GenericMenu GetMenu(Type matchType, GenericMenu.MenuFunction2 func = default, bool autoConvert = true)
        {
            GenericMenu bindMenu = new GenericMenu();
            //bindMenu.AddItem(new GUIContent("A"), false,
            //    () =>
            //    {
            //        Debug.Log(1);
            //    });
            //bindMenu.AddItem(new GUIContent("Test"), false,
            //    () =>
            //    {
            //        Debug.Log(1);
            //    });

            var componentTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Where(t => typeof(Component).IsAssignableFrom(t))
                .Where(t => t.IsPublic).ToList();

            componentTypes.Add(typeof(GameObject));
            componentTypes = componentTypes.OrderBy(t => t.Name).ToList();

            foreach (var type in componentTypes)
            {
                var ms = GetMembers(type, matchType);
                foreach (var member in ms)
                {
                    List<MemberInfo> members = new List<MemberInfo>();
                    members.Add(type);
                    members.Add(member.Member);

                    var FirstC = type.Name[0].ToString().ToUpper();
                    //类型名放前面 自动转换时会导致 类型名长度不一样
                    if (true || member.Member.DeclaringType == type) //暂时不显示[Inherited]
                    {
                        bindMenu.AddItem(new GUIContent($"{FirstC}/{type.Name}/{member.Name} : [{member.ValueType.Name}]"), false, func, members);
                    }
                    else
                    {
                        bindMenu.AddItem(new GUIContent($"{FirstC}/{type.Name}/[Inherited]: {member.Name} : [{member.ValueType.Name}]"), false, func, members);
                    }
                }
            }
            return bindMenu;
        }

        public class MItem
        {
            public string Name { get; set; }
            public List<MemberInfo> Members { get; internal set; }
        }

        public class MyMember
        {
            public string Name { get; internal set; }
            public MemberInfo Member { get; internal set; }
            public Type ValueType { get; internal set; }
        }

        public static IOrderedEnumerable<MyMember> GetMembers(Type type, Type matchType)
        {
            var fields = type.GetFields().Where(f =>
            {
                if (matchType == null)
                {
                    return true;
                }
                return matchType.IsAssignableFrom(f.FieldType);
            });

            var allf = from f in fields
                       select new MyMember() { Name = f.Name, Member = f, ValueType = f.FieldType };

            var propertie = type.GetProperties().Where(f =>
            {
                if (matchType == null)
                {
                    return true;
                }
                return matchType.IsAssignableFrom(f.PropertyType);
            });

            var allPorp = from p in propertie
                          select new MyMember() { Name = p.Name, Member = p, ValueType = p.PropertyType };
            //var methods = type.GetMethods().Where(MatchMethod<To>).Cast<MemberInfo>();
            return allf.Concat(allPorp).OrderBy(m => m.Name)/*.Concat(methods)*/;
        }

        public static bool MatchMethod(MethodInfo method, Type matchType)
        {
            bool rType = matchType.IsAssignableFrom(method.ReturnType);
            if (!rType)
            {
                return false;
            }

            var p = method.GetParameters();
            if (p == null || p.Length == 0)
            {
                //参数个数小于0
            }
            else
            {
                return false;
            }

            return true;
        }
    }

#endif
}

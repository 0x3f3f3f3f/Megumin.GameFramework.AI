using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using System.Reflection;
using UnityEngine.UIElements;
using Megumin.Reflection;
using UnityEngine.Profiling;

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
            //settingButton.tooltip = "Set BindingPath";
            //settingButton.style.backgroundImage = settingIcon.image as Texture2D;

            var gType = fieldInfo.DeclaringType.GetGenericArguments()[0];
            var targetObj = property.serializedObject.targetObject;

            settingButton.clickable.activators.Add(new ManipulatorActivationFilter
            {
                button = MouseButton.RightMouse
            });

            settingButton.clickable.clickedWithEventInfo += async (evt) =>
            {
                if (evt is IMouseEvent mouseEvent)
                {
                    bindingOptions.Button = mouseEvent.button;
                    SetOptionGO(targetObj);

                    var path = await BindingEditor.GetBindStr(gType, bindingOptions);

                    //TODO, Editor hasunsavechange
                    //Undo.RecordObject(property.serializedObject.targetObject, $"Set BindingPath {property.propertyPath}");
                    //Debug.Log(path + "-----------------");
                    //property.SetValue(path);

                    //此时已经无法给property赋值了。改为给TextField赋值。
                    var textField = field.Q<TextField>();
                    textField.value = path;
                }
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

        public static readonly BindingOptions bindingOptions = new();
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

            bool click = GUI.Button(buttonPosition, settingIcon);

            if (click)
            {
                bindingOptions.Button = Event.current.button;
                SetOptionGO(property.serializedObject.targetObject);
            }

            if (property.GetBindintString(click, out var str, gType, bindingOptions))
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

        protected static void SetOptionGO(UnityEngine.Object @object)
        {
            if (@object is GameObject go && go)
            {
                bindingOptions.GameObject = go;
            }
            else if (@object is Component component && component)
            {
                bindingOptions.GameObject = component.gameObject;
            }
            else if (Selection.activeGameObject)
            {
                bindingOptions.GameObject = Selection.activeGameObject;
            }
            else
            {
                bindingOptions.GameObject = null;
            }
        }
    }

    public class BindingOptions
    {
        public GameObject GameObject { get; set; }
        public bool UseAdpter { get; set; } = true;
        public bool StaticMember { get; set; } = true;
        public bool InterfaceMember { get; set; } = true;
        public bool GameObjectTypeMember { get; set; } = true;
        public bool CompmentTypeMember { get; set; } = true;
        public bool CustomTypeMember { get; set; } = false;
        public bool InheritMember { get; set; } = true;
        public int Button { get; set; } = 0;
    }


    public static class BindingEditor
    {
        public static bool GetBindintString(this SerializedProperty property,
                                            bool click,
                                            out string str,
                                            Type matchType = null,
                                            BindingOptions options = null)
        {
            if (click)
            {
                BindingEditor.GetBindStr(property.propertyPath, matchType, options);
            }

            if (BindingEditor.cacheResult.TryGetValue(property.propertyPath, out str))
            {
                BindingEditor.cacheResult.Remove(property.propertyPath);
                return true;
            }
            str = default;
            return false;
        }

        private const BindingFlags BindingAttr = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance;
        public static Dictionary<string, string> cacheResult = new Dictionary<string, string>();
        public static async void GetBindStr(string propertyPath)
        {
            var str = await BindingEditor.GetBindStr<int>();
            cacheResult[propertyPath] = str;
        }

        public static async void GetBindStr(string propertyPath, Type matchType, BindingOptions options = null)
        {
            var str = await BindingEditor.GetBindStr(matchType, options);
            cacheResult[propertyPath] = str;
        }


        public static Task<string> GetBindStr<T>()
        {
            return GetBindStr(typeof(T));
        }

        public static Task<string> GetBindStr(Type matchType, BindingOptions options = null)
        {
            TaskCompletionSource<string> source = new TaskCompletionSource<string>();
            GenericMenu.MenuFunction2 func = path =>
            {
                source.SetResult((string)path);
            };

            GenericMenu bindMenu = GetMenu2(matchType, func, options);
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
                //.Where(t => typeof(Component).IsAssignableFrom(t))
                .Where(t => t.IsPublic).ToList();

            componentTypes.Add(typeof(GameObject));
            componentTypes = componentTypes.OrderBy(t => t.Name).ToList();

            foreach (var type in componentTypes)
            {
                var ms = GetMembers(type, matchType);
                foreach (var member in ms)
                {
                    var resultPath = $"{type.FullName}/{member.Name}";

                    var FirstC = type.Name[0].ToString().ToUpper();

                    //类型名放前面 自动转换时会导致 类型名长度不一样
                    if (true || member.Member.DeclaringType == type) //暂时不显示[Inherited]
                    {
                        bindMenu.AddItem(new GUIContent($"{FirstC}/{type.Name}/{member.Name} : [{member.ValueType.Name}]"),
                                         false,
                                         func,
                                         resultPath);
                    }
                    else
                    {
                        bindMenu.AddItem(new GUIContent($"{FirstC}/{type.Name}/[Inherited]: {member.Name} : [{member.ValueType.Name}]"),
                                         false,
                                         func,
                                         resultPath);
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

        public static bool IsUnityComp(Type type)
        {
            if (type == typeof(GameObject))
            {
                return true;
            }

            return typeof(Component).IsAssignableFrom(type);
        }

        public static IOrderedEnumerable<MyMember> GetMembers(Type type, Type matchType)
        {
            var fields = type.GetFields(BindingAttr).Where(f =>
            {
                if (f.IsStaticMember() == false && IsUnityComp(type) == false)
                {
                    return false;
                }

                if (matchType == null)
                {
                    return true;
                }
                return matchType.IsAssignableFrom(f.FieldType);
            });

            var allf = from f in fields
                       select new MyMember() { Name = f.Name, Member = f, ValueType = f.FieldType };

            var propertie = type.GetProperties(BindingAttr).Where(f =>
            {
                if (f.IsStaticMember() == false && IsUnityComp(type) == false)
                {
                    return false;
                }

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



        public class MyItem : IComparable<MyItem>
        {
            public string BindPath;

            //MainGUIContent 和 InhertGUIContent 是多线程生成的。
            //这里是使用空间换时间，所以没有使用延迟初始化。
            public GUIContent InhertGUIContent;
            public GUIContent MainGUIContent;
            public GUIContent NoFirstCGUIContent;

            /// <summary>
            /// 所属类型
            /// </summary>
            public Type Type { get; internal set; }
            /// <summary>
            /// 目标值类型
            /// </summary>
            public Type ValueType { get; internal set; }
            public MemberInfo Member { get; internal set; }
            public bool IsInherit { get; internal set; }
            public string FirstC { get; internal set; }
            public bool IsStatic { get; internal set; }

            public int CompareTo(MyItem other)
            {
                return MainGUIContent.text.CompareTo(other.MainGUIContent.text);
            }
        }

        static readonly Dictionary<Type, (List<MyItem> matchTypeResult, List<MyItem> typeAdpterResult)> cache = new();
        static readonly Unity.Profiling.ProfilerMarker GetAllItemMarker = new("GetAllItem");

        public static void GetAllItem(Type target,
                                      out List<MyItem> targetResult,
                                      out List<MyItem> adpterResult)
        {
            using var profiler = GetAllItemMarker.Auto();

            if (cache.TryGetValue(target, out var result))
            {
                targetResult = result.matchTypeResult;
                adpterResult = result.typeAdpterResult;
            }
            else
            {
                GetMyItems(target, out targetResult, out adpterResult);
                cache[target] = (targetResult, adpterResult);
            }
        }

        public static void GetMyItems(Type target,
                                      out List<MyItem> targetResult,
                                      out List<MyItem> adpterResult)
        {
            EditorUtility.DisplayProgressBar("CacheMenuItem", target.FullName, 0.75f);

            var matchTypeResult1 = new List<MyItem>();
            var typeAdpterResult1 = new List<MyItem>();

            var allTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                //.Where(t => typeof(Component).IsAssignableFrom(t))
                .Where(t => t.IsPublic).ToList();

            var adpterTypes = new HashSet<Type>();

            foreach (var item in TypeAdpter.Adpters)
            {
                if (item.Key.Item2 == target)
                {
                    adpterTypes.Add(item.Key.Item1);
                }
            }

            List<Task> tasks = new();
            for (int i = 0; i < allTypes.Count; i++)
            {
                Type type = allTypes[i];
                //EditorUtility.DisplayProgressBar("CacheMenuItem", type.FullName, (float)i / allTypes.Count);
                var task = Task.Run(() => { GetTypeItems(target, adpterTypes, matchTypeResult1, typeAdpterResult1, type); });
                tasks.Add(task);
                //task.Wait(); //测试时单线程，方便断点
            }

            Task.WaitAll(tasks.ToArray());
            EditorUtility.ClearProgressBar();

            matchTypeResult1.Sort();
            typeAdpterResult1.Sort();

            targetResult = matchTypeResult1;
            adpterResult = typeAdpterResult1;
        }

        public static void GetTypeItems(Type target,
                                        HashSet<Type> adpterTypes,
                                        List<MyItem> targetResult,
                                        List<MyItem> adpterResult,
                                        Type type)
        {
            var FirstC = type.Name[0].ToString().ToUpper();

            var fields = type.GetFields(BindingAttr).Where(f =>
            {
                if (f.IsStaticMember() == false
                    && IsUnityComp(type) == false
                    && type.IsInterface == false)
                {
                    return false;
                }

                if (adpterTypes.Contains(f.FieldType))
                {
                    return true;
                }

                if (target == null)
                {
                    return true;
                }
                return target.IsAssignableFrom(f.FieldType);
            }).ToList();

            foreach (var member in fields)
            {
                MyItem item = new();
                item.Type = type;
                item.BindPath = $"{type.FullName}/{member.Name}";
                item.ValueType = member.GetMemberType();
                item.Member = member;
                item.IsInherit = member.DeclaringType != type;
                item.FirstC = FirstC;
                item.IsStatic = member.IsStaticMember();

                var staticTag = item.IsStatic ? " [S]" : "";

                item.MainGUIContent = new($"{FirstC}/{type.Name}/{member.Name} : [{item.ValueType.Name}]{staticTag}");
                item.InhertGUIContent = new($"{FirstC}/{type.Name}/[Inherited]: {member.Name} : [{item.ValueType.Name}]{staticTag}");
                item.NoFirstCGUIContent = new($"{type.Name}/{member.Name} : [{item.ValueType.Name}]{staticTag}");

                if (item.ValueType == target)
                {
                    targetResult.Add(item);
                }
                else
                {
                    adpterResult.Add(item);
                }
            }

            var props = type.GetProperties(BindingAttr).Where(f =>
            {
                if (f.IsStaticMember() == false
                    && IsUnityComp(type) == false
                    && type.IsInterface == false)
                {
                    return false;
                }

                if (adpterTypes.Contains(f.PropertyType))
                {
                    return true;
                }

                if (target == null)
                {
                    return true;
                }
                return target.IsAssignableFrom(f.PropertyType);
            }).ToList();

            foreach (var member in props)
            {
                MyItem item = new();
                item.Type = type;
                item.BindPath = $"{type.FullName}/{member.Name}";
                item.ValueType = member.GetMemberType();
                item.Member = member;
                item.IsInherit = member.DeclaringType != type;
                item.FirstC = FirstC;
                item.IsStatic = member.IsStaticMember();

                var staticTag = item.IsStatic ? " [S]" : "";

                item.MainGUIContent = new($"{FirstC}/{type.Name}/{member.Name} : [{item.ValueType.Name}]{staticTag}");
                item.InhertGUIContent = new($"{FirstC}/{type.Name}/[Inherited]: {member.Name} : [{item.ValueType.Name}]{staticTag}");
                item.NoFirstCGUIContent = new($"{type.Name}/{member.Name} : [{item.ValueType.Name}]{staticTag}");

                if (item.ValueType == target)
                {
                    targetResult.Add(item);
                }
                else
                {
                    adpterResult.Add(item);
                }
            }
        }


        static Dictionary<Type, Menu> cacheMenu = new();
        static readonly Unity.Profiling.ProfilerMarker GetMenu2Marker = new("GetMenu2");
        public static GenericMenu GetMenu2(Type target,
                                           GenericMenu.MenuFunction2 func = default,
                                           BindingOptions options = null)
        {
            using var profiler = GetMenu2Marker.Auto();
            if (cacheMenu.TryGetValue(target, out var menu))
            {

            }
            else
            {
                menu = new Menu(target);
                cacheMenu[target] = menu;
            }

            var result = menu.CreateMenu(options, func);
            return result;
        }

        public class Menu
        {
            public GenericMenu BindMenu { get; private set; }

            public List<MyItem> ItemList { get; private set; }
            public List<MyItem> TargetResult { get; private set; }
            public List<MyItem> AdpterResult { get; private set; }

            public Menu(Type target)
            {
                MatchType = target;

                ItemList = new List<MyItem>();

                GetAllItem(target, out var targetResult, out var adpterResult);

                TargetResult = targetResult;
                AdpterResult = adpterResult;

                ItemList.AddRange(targetResult);
                ItemList.AddRange(adpterResult);
                ItemList.Sort();
            }

            private void Func(object userData)
            {
                Callback?.Invoke(userData);
            }

            public GenericMenu CreateMenu(BindingOptions options, GenericMenu.MenuFunction2 func)
            {
                if (options != null && options.Button == 0)
                {
                    var result = new GenericMenu();

                    var list = TargetResult;
                    if (options.UseAdpter)
                    {
                        list = ItemList;
                    }

                    HashSet<Type> hasCompType = new()
                    {
                        typeof(GameObject),
                        typeof(Transform),
                    };

                    var go = options.GameObject;
                    if (go)
                    {
                        var copms = go.GetComponents<Component>();
                        foreach (var item in copms)
                        {
                            hasCompType.Add(item.GetType());
                        }
                    }

                    // 仅go含有的组件和静态成员
                    foreach (var item in list)
                    {
                        if (item != null)
                        {
                            if (hasCompType.Contains(item.Type) /*|| item.IsStatic*/)
                            {
                                result.AddItem(item.NoFirstCGUIContent, false, func, item.BindPath);
                            }
                        }
                    }

                    return result;
                }
                else
                {
                    if (BindMenu == null)
                    {
                        BindMenu = new GenericMenu();
                        foreach (var item in ItemList)
                        {
                            if (item != null)
                            {
                                BindMenu.AddItem(item.MainGUIContent, false, Func, item.BindPath);
                            }
                        }
                    }

                    Callback = func;
                    return BindMenu;
                }
            }

            public Type MatchType { get; }
            public GenericMenu.MenuFunction2 Callback { get; internal set; }
        }

    }

#endif
}

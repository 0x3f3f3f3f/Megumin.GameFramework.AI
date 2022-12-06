using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using static Megumin.Binding.Editor.BindingEditor;
using static UnityEditor.Progress;

namespace Megumin.Binding.Editor
{
    public class BindingEditor
    {
        public static Task<string> GetBindStr<T>()
        {
            return GetBindStr(typeof(T));
        }

        public static Task<string> GetBindStr(Type matchType)
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

            GenericMenu bindMenu = GetMenu(typeof(int), func);
            bindMenu.ShowAsContext();
            return source.Task;
        }

        public static GenericMenu GetMenu(Type matchType, GenericMenu.MenuFunction2 func = default)
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
                    if (member.Member.DeclaringType == type)
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
            var fields = type.GetFields().Where(f => matchType.IsAssignableFrom(f.FieldType));
            var allf = from f in fields
                       select new MyMember() { Name = f.Name, Member = f , ValueType = f.FieldType};
            var propertie = type.GetProperties().Where(f => matchType.IsAssignableFrom(f.PropertyType));

            var  allPorp = from p in propertie
                           select new MyMember() { Name = p.Name, Member = p, ValueType = p.PropertyType };
            //var methods = type.GetMethods().Where(MatchMethod<T>).Cast<MemberInfo>();
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
}

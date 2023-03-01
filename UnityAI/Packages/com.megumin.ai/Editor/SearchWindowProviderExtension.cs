using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Megumin.GameFramework.AI.Editor
{
    public static class SearchWindowProviderExtension
    {
        /// <summary>
        /// 添加 Category 特性
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="provider"></param>
        /// <param name="tree"></param>
        /// <param name="levelOffset"></param>
        public static void AddCateGory<T>(this ISearchWindowProvider provider,
                                              List<SearchTreeEntry> tree,
                                              int levelOffset = 1)
        {
            var types = TypeCache.GetTypesDerivedFrom<T>();
            var pairs = from type in types
                        let attri = type.GetCustomAttribute<CategoryAttribute>()
                        where attri != null
                        select (attri, type);

            HashSet<string> alreadyAddPathName = new();

            AddGroupToTree(tree, levelOffset, pairs, alreadyAddPathName);
        }

        public static void AddGroupToTree(List<SearchTreeEntry> tree, int levelOffset, IEnumerable<(CategoryAttribute attri, Type type)> group, HashSet<string> alreadyAddPathName)
        {
            foreach (var item in group.OrderBy(elem => elem.attri.Category)) //额外排序一次
            {
                var type = item.type;
                var levelString = item.attri.Category.Split('/');
                var pathName = "";
                for (int i = 0; i < levelString.Length; i++)
                {
                    var levelName = levelString[i];
                    pathName += levelName;
                    if (!alreadyAddPathName.Contains(pathName))
                    {
                        alreadyAddPathName.Add(pathName);
                        tree.Add(new SearchTreeGroupEntry(new GUIContent(levelName)) { level = levelOffset + i });
                    }
                }

                GUIContent content = new($"      {type.Name}", image: null);
                SearchTreeEntry entry = new(content)
                {
                    level = levelOffset + levelString.Length,
                    userData = type
                };
                tree.Add(entry);
            }
        }

        /// <summary>
        /// 添加 Category 特性
        /// 子Group排在子项上面
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="provider"></param>
        /// <param name="tree"></param>
        /// <param name="levelOffset"></param>
        public static void AddCateGory2<T>(this ISearchWindowProvider provider,
                                              List<SearchTreeEntry> tree,
                                              int levelOffset = 1)
        {
            var types = TypeCache.GetTypesDerivedFrom<T>();
            var pairs = from type in types
                        let attri = type.GetCustomAttribute<CategoryAttribute>()
                        where attri != null
                        orderby attri.Category
                        //orderby type.Name  //不知道为什么会导致排序不正确
                        group (attri, type) by attri.Category;

            var groups = pairs.ToList();

            HashSet<string> alreadyAddPathName = new();
            HashSet<int> alreadyAddGroup = new();
            void AddGroup(int index)
            {
                if (index >= groups.Count)
                {
                    return;
                }

                var curGroup = groups[index];
                if (index + 1 < groups.Count)
                {
                    var nextGroup = groups[index + 1];
                    if (nextGroup.Key.Contains(curGroup.Key) && nextGroup.Key != curGroup.Key)
                    {
                        //如果下一个组是这个组得子组，优先添加
                        AddGroup(index + 1);
                    }
                }

                //添加到tree
                if (!alreadyAddGroup.Contains(index))
                {
                    alreadyAddGroup.Add(index);
                    AddGroupToTree(tree, levelOffset, curGroup, alreadyAddPathName);
                }
            }

            for (int i = 0; i < groups.Count; i++)
            {
                AddGroup(i);
            }
        }
    }
}





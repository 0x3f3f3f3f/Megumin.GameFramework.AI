using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Megumin.Binding;
using Megumin.Reflection;
using Megumin.Serialization;
using UnityEngine;
using UnityEngine.Serialization;

namespace Megumin.GameFramework.AI.BehaviorTree
{
    [CreateAssetMenu(fileName = "BehaviorTree", menuName = "Megumin/AI/BehaviorTreeAsset")]
    public partial class BehaviorTreeAsset_1_1 : ScriptableObject, IBehaviorTreeAsset
    {
        public string Version = new Version(1, 1, 0).ToString();
        [field: ContextMenuItem("ChangeGUID", "ChangeGUID")]
        [field: SerializeField]
        public string GUID { get; set; } = Guid.NewGuid().ToString();
        [field: SerializeField]
        public string StartNodeGUID { get; set; } = "";
        public bool UseSerializeReferenceGeneric = false;
        public List<UnityObjectData> UnityObjectRef;
        public List<ObjectData> variables;
        public List<ObjectData> nodes;
        public List<ObjectData> decorators;
        [FormerlySerializedAs("treeElements")]
        public List<ObjectData> refObjs;

        public string TreeName { get; protected set; }

        private void OnEnable()
        {
            //name只能在OnEnable被调用
            TreeName = name;
        }

        public bool SaveTree(BehaviorTree tree)
        {
            if (tree == null)
            {
                return false;
            }

            SharedMeta.Clear();

            if (!Guid.TryParse(tree.GUID, out var _))
            {
                tree.GUID = Guid.NewGuid().ToString();
            }
            GUID = tree.GUID;
            StartNodeGUID = tree.StartNode?.GUID;

            //回调
            foreach (var node in tree.AllNodes)
            {
                if (node is ISerializationCallbackReceiver nodeCallback)
                {
                    nodeCallback.OnBeforeSerialize();
                }

                foreach (var decorator in node.Decorators)
                {
                    if (decorator is ISerializationCallbackReceiver decoratorCallback)
                    {
                        decoratorCallback.OnBeforeSerialize();
                    }
                }
            }

            Dictionary<object, string> cacheRef = new();
            Stack<(string name, object value)> needSerialization = new();
            List<UnityObjectData> objRefs = new();

            //缓存所有已知引用对象
            cacheRef.Add(tree, tree.GUID);

            foreach (var variable in tree.Variable.Table)
            {
                cacheRef.Add(variable, variable.RefName);
            }

            foreach (var node in tree.AllNodes)
            {
                cacheRef.Add(node, node.GUID);

                foreach (var decorator in node.Decorators)
                {
                    cacheRef.Add(decorator, decorator.GUID);
                }
            }

            {
                //序列化参数表
                List<ObjectData> variableDatas = new();

                foreach (var variable in tree.Variable.Table)
                {
                    ObjectData data = new ObjectData();
                    if (data.TrySerialize(variable.RefName, variable, needSerialization, objRefs, cacheRef, GetSerializeMembers))
                    {
                        variableDatas.Add(data);
                    }
                }

                //参数表保持顺序
                //variableDatas.Sort();
                variables = variableDatas;
            }

            {
                //序列化节点
                List<ObjectData> nodeDatas = new();
                List<ObjectData> decoratorDatas = new();

                void SaveNode(BTNode node)
                {
                    ObjectData nodeData = new ObjectData();
                    if (nodeData.TrySerialize(node.GUID, node, needSerialization, objRefs, cacheRef, GetSerializeMembers))
                    {
                        nodeDatas.Add(nodeData);
                    }
                }

                void SaveDeco(ITreeElement decorator)
                {
                    ObjectData decoratorData = new ObjectData();
                    if (decoratorData.TrySerialize(decorator.GUID, decorator, needSerialization, objRefs, cacheRef, GetSerializeMembers))
                    {
                        decoratorDatas.Add(decoratorData);
                    }
                }

                foreach (var node in tree.AllNodes)
                {
                    SaveNode(node);

                    foreach (var decorator in node.Decorators)
                    {
                        SaveDeco(decorator);
                    }
                }

                nodeDatas.Sort();
                nodes = nodeDatas;

                decoratorDatas.Sort();
                decorators = decoratorDatas;

                List<ObjectData> refObjsDatas = new();
                while (needSerialization.Count > 0)
                {
                    var item = needSerialization.Pop();
                    ObjectData data = new ObjectData();
                    if (data.TrySerialize(item.name, item.value, needSerialization, objRefs, cacheRef, GetSerializeMembers))
                    {
                        refObjsDatas.Add(data);
                    }
                }

                refObjsDatas.Sort();
                refObjs = refObjsDatas;
            }


            UnityObjectRef = objRefs;
            return true;


        }

        /// <summary>
        /// 装饰器，子节点直接引用序列化，自动关联引用。不用额外处理
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance"></param>
        /// <returns></returns>
        public IEnumerable<InstanceMemberInfo>
            GetSerializeMembers<T>(T instance)
        {
            foreach (var item in instance.GetSerializeMembers())
            {
                if (instance is BehaviorTreeElement treeElement)
                {
                    if (item.Value == treeElement.Tree)
                    {
                        continue;
                    }
                }
                yield return item;
            }
        }

        public UnityEngine.Object AssetObject => this;

        public ConcurrentDictionary<string, object> SharedMeta { get; } = new();

        static readonly Unity.Profiling.ProfilerMarker instantiateMarker = new("Instantiate");
        public BehaviorTree Instantiate(InitOption initOption, IRefFinder refFinder = null)
        {
            using var profiler = instantiateMarker.Auto();

            if (initOption == null)
            {
                return null;
            }


            RefFinder finder = new();
            finder.Parent = refFinder;

            //缓存Unity引用对象
            if (UnityObjectRef != null)
            {
                foreach (var item in UnityObjectRef)
                {
                    finder.RefDic.Add(item.Name, item.Ref);
                }
            }

            if (initOption.UseGenerateCode)
            {
                //使用生成的代码实例化行为树。
                BehaviorTreeCreator creator = BehaviorTreeCreator.GetCreator(TreeName, GUID, null);
                if (creator == null)
                {
                    Debug.LogWarning($"{TreeName} Code Creator can not found. Fallback default Instantiate.");
                }
                else
                {
                    var codeTree = creator.Instantiate(initOption, finder);
                    codeTree.Asset = this;
                    return codeTree;
                }
            }

            BehaviorTree tree = new();
            tree.GUID = GUID;
            tree.InstanceGUID = Guid.NewGuid().ToString();
            if (!Guid.TryParse(tree.GUID, out var _))
            {
                tree.GUID = Guid.NewGuid().ToString();
            }

            tree.Asset = this;
            tree.RootTree = tree;
            tree.InitOption = initOption;
            tree.RefFinder = finder;

            if (UseSerializeReferenceGeneric)
            {
                //多态序列化 + 泛型
                //
                Debug.Log("至少需要unity2023");
                return tree;
            }

            //先创建引用实例

            //创建参数表实例
            Dictionary<ObjectData, object> variableCache = new();
            if (variables != null)
            {
                foreach (var item in variables)
                {
                    if (string.IsNullOrEmpty(item.Name))
                    {
                        Debug.LogError($"意外错误，没有引用名字");
                        continue;
                    }
                    if (item.TryCreateInstance(out var instance))
                    {
                        finder.RefDic.Add(item.Name, instance);
                        variableCache.Add(item, instance);
                    }
                }
            }


            //创建节点实例
            Dictionary<ObjectData, object> nodeObjCache = new();
            if (nodes != null)
            {
                foreach (var item in nodes)
                {
                    if (string.IsNullOrEmpty(item.Name))
                    {
                        Debug.LogError($"意外错误，没有引用名字");
                        continue;
                    }

                    if (item.TryCreateInstance(out var instance))
                    {

                    }
                    else
                    {
                        //使用MissingNode代替
                        MissingNode missing = new();
                        missing.MissType = item.Type;
                        missing.GUID = item.Name;
                        missing.OrignalData = item;
                        instance = missing;
                    }

                    finder.RefDic.Add(item.Name, instance);
                    nodeObjCache.Add(item, instance);
                }
            }

            //创建装饰器实例
            Dictionary<ObjectData, object> decoratorObjCache = new();
            if (decorators != null)
            {
                foreach (var item in decorators)
                {
                    if (string.IsNullOrEmpty(item.Name))
                    {
                        Debug.LogError($"意外错误，没有引用名字");
                        continue;
                    }

                    if (item.TryCreateInstance(out var instance))
                    {

                    }
                    else
                    {
                        //使用MissingDecorator代替
                        MissingDecorator missing = new();
                        missing.MissType = item.Type;
                        missing.GUID = item.Name;
                        missing.OrignalData = item;
                        instance = missing;
                    }

                    finder.RefDic.Add(item.Name, instance);
                    decoratorObjCache.Add(item, instance);
                }
            }

            //创建引用对象实例
            Dictionary<ObjectData, object> treeObjCache = new();
            if (refObjs != null)
            {
                foreach (var item in refObjs)
                {
                    if (string.IsNullOrEmpty(item.Name))
                    {
                        Debug.LogError($"意外错误，没有引用名字");
                        continue;
                    }

                    if (item.TryCreateInstance(out var instance))
                    {
                        if (initOption.SharedMeta && instance is IAIMeta)
                        {
                            if (SharedMeta.TryGetValue(item.Name, out var shared))
                            {
                                //使用共享的meta实例。
                                finder.RefDic.Add(item.Name, shared);
                            }
                            else
                            {
                                finder.RefDic.Add(item.Name, instance);
                                treeObjCache.Add(item, instance);
                            }
                        }
                        else
                        {
                            finder.RefDic.Add(item.Name, instance);
                            treeObjCache.Add(item, instance);
                        }
                    }
                }
            }

            finder.RefDic.Add(tree.GUID, tree);

            //反序列化
            //反序列化参数表
            foreach (var item in variableCache)
            {
                object value = item.Value;
                if (item.Key.TryDeserialize(value, finder))
                {
                    tree.InitAddVariable(value);
                }
            }

            void DeserializeObj(Dictionary<ObjectData, object> cache)
            {
                foreach (var item in cache)
                {
                    object value = item.Value;
                    if (item.Key.TryDeserialize(value, finder))
                    {
                        if (value is IAIMeta meta)
                        {
                            if (SharedMeta.ContainsKey(item.Key.Name) == false)
                            {
                                SharedMeta[item.Key.Name] = meta;
                            }
                        }

                        tree.InitAddTreeRefObj(value);
                    }
                }
            }

            //反序列化树节点 节点父子关系和装饰器关系自动关联
            //先反序列化引用对象，在反序列化装饰，最后反序列化节点。
            //这样可以尽量保证SetMemberByAttribute调用时，引用对象已经反序列化完毕。
            DeserializeObj(treeObjCache);
            DeserializeObj(decoratorObjCache);
            DeserializeObj(nodeObjCache);

            //设置开始节点 装饰器Owner
            foreach (var node in tree.AllNodes)
            {
                if (node.GUID == StartNodeGUID)
                {
                    tree.StartNode = node;
                }

                foreach (var decorator in node.Decorators)
                {
                    decorator.Owner = node;
                }
            }

            tree.UpdateNodeIndexDepth();

            if (initOption.LazyInitSubtree)
            {

            }
            else
            {
                foreach (var item in tree.AllNodes)
                {
                    if (item is SubTree subtreeNode)
                    {
                        subtreeNode.BehaviourTree
                            = tree.InstantiateSubTree(subtreeNode.BehaviorTreeAsset, subtreeNode);
                    }
                }
            }

            //回调
            foreach (var node in tree.AllNodes)
            {
                if (node is ISerializationCallbackReceiver nodeCallback)
                {
                    nodeCallback.OnAfterDeserialize();
                }

                foreach (var decorator in node.Decorators)
                {
                    if (decorator is ISerializationCallbackReceiver decoratorCallback)
                    {
                        decoratorCallback.OnAfterDeserialize();
                    }
                }
            }

            return tree;
        }

        [Editor]
        public void ChangeGUID()
        {
            GUID = Guid.NewGuid().ToString();
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
            UnityEditor.AssetDatabase.SaveAssetIfDirty(this);
#endif
        }
    }

}



﻿///********************************************************************************************************************************
///The code on this page is generated by the code generator, do not manually modify.
///CodeGenerator: Megumin.CSCodeGenerator.  Version: 1.0.2
///********************************************************************************************************************************

using System;
using Megumin.Reflection;
using Megumin.Serialization;

namespace Megumin.AI.BehaviorTree
{
    public sealed partial class BT_WaitAndLog_71ad0dfc_da1a_4b3c_b95d_b4276312641a_Creator : BehaviorTreeCreator
    {
        static readonly Unity.Profiling.ProfilerMarker instantiateMarker = new("WaitAndLog_Init");
        public override BehaviorTree Instantiate(InitOption initOption, IRefFinder refFinder = null)
        {
            using var profiler = instantiateMarker.Auto();

            if (initOption == null)
            {
                return null;
            }

            //创建 引用查找器
            RefFinder finder = new();
            finder.Parent = refFinder;

            //创建 树实例
            BehaviorTree tree = new();
            tree.GUID = "71ad0dfc-da1a-4b3c-b95d-b4276312641a";
            tree.RootTree = tree;
            tree.InitOption = initOption;
            tree.RefFinder = finder;

            var tree_71ad0dfc_da1a_4b3c_b95d_b4276312641a = tree;

            //创建 参数，节点，装饰器，普通对象

            var node_2dfe0c27_6bca_4af7_ba6b_a1631d859f78 = new Megumin.AI.BehaviorTree.Wait();
            var temp_2dfe0c27_6bca_4af7_ba6b_a1631d859f78_Meta = new Megumin.AI.BehaviorTree.NodeMeta();
            var temp_2dfe0c27_6bca_4af7_ba6b_a1631d859f78_Decorators = new System.Collections.Generic.List<Megumin.AI.BehaviorTree.IDecorator>();
            var temp_2dfe0c27_6bca_4af7_ba6b_a1631d859f78_WaitTime = new Megumin.Binding.RefVar_Float();

            var node_2f7d3ba7_cfbb_416a_a003_e1d3ed6f0707 = new Megumin.AI.BehaviorTree.Log();
            var temp_2f7d3ba7_cfbb_416a_a003_e1d3ed6f0707_Meta = new Megumin.AI.BehaviorTree.NodeMeta();
            var temp_2f7d3ba7_cfbb_416a_a003_e1d3ed6f0707_Decorators = new System.Collections.Generic.List<Megumin.AI.BehaviorTree.IDecorator>();
            var temp_2f7d3ba7_cfbb_416a_a003_e1d3ed6f0707_Info = new Megumin.AI.LogInfo();
            var temp_2f7d3ba7_cfbb_416a_a003_e1d3ed6f0707_Info_Text = new Megumin.Binding.RefVar_String();
            var temp_2f7d3ba7_cfbb_416a_a003_e1d3ed6f0707_Info_Ref_Transform = new Megumin.Binding.RefVar_Transform();
            var temp_2f7d3ba7_cfbb_416a_a003_e1d3ed6f0707_Info_Ref_GameObject = new Megumin.Binding.RefVar_GameObject();

            var node_46091aac_2293_44bf_881e_5e3476886543 = new Megumin.AI.BehaviorTree.WaitDo();
            var temp_46091aac_2293_44bf_881e_5e3476886543_Meta = new Megumin.AI.BehaviorTree.NodeMeta();
            var temp_46091aac_2293_44bf_881e_5e3476886543_Decorators = new System.Collections.Generic.List<Megumin.AI.BehaviorTree.IDecorator>();
            var temp_46091aac_2293_44bf_881e_5e3476886543_Children = new System.Collections.Generic.List<Megumin.AI.BehaviorTree.BTNode>();
            var temp_46091aac_2293_44bf_881e_5e3476886543_WaitTime = new Megumin.Binding.RefVar_Float();

            var node_62f41474_7568_45c7_946e_cdf5ad45fff4 = new Megumin.AI.BehaviorTree.Wait();
            var temp_62f41474_7568_45c7_946e_cdf5ad45fff4_Meta = new Megumin.AI.BehaviorTree.NodeMeta();
            var temp_62f41474_7568_45c7_946e_cdf5ad45fff4_Decorators = new System.Collections.Generic.List<Megumin.AI.BehaviorTree.IDecorator>();
            var temp_62f41474_7568_45c7_946e_cdf5ad45fff4_WaitTime = new Megumin.Binding.RefVar_Float();

            var node_a734621c_96b6_4a26_898c_0e4d45f5dba2 = new Megumin.AI.BehaviorTree.Sequence();
            var temp_a734621c_96b6_4a26_898c_0e4d45f5dba2_Meta = new Megumin.AI.BehaviorTree.NodeMeta();
            var temp_a734621c_96b6_4a26_898c_0e4d45f5dba2_Decorators = new System.Collections.Generic.List<Megumin.AI.BehaviorTree.IDecorator>();
            var temp_a734621c_96b6_4a26_898c_0e4d45f5dba2_Children = new System.Collections.Generic.List<Megumin.AI.BehaviorTree.BTNode>();
            var deco_f215473e_1bd7_451b_9d38_79a052d9b203 = new Megumin.AI.BehaviorTree.Loop_Decorator();

            //以上创建 0 参数
            //以上创建 5 节点
            //以上创建 1 装饰器
            //以上创建 19 普通对象

            //以上创建 25 所有对象

            finder.RefDic.Add("2dfe0c27-6bca-4af7-ba6b-a1631d859f78", node_2dfe0c27_6bca_4af7_ba6b_a1631d859f78);
            finder.RefDic.Add("2dfe0c27-6bca-4af7-ba6b-a1631d859f78.Meta", temp_2dfe0c27_6bca_4af7_ba6b_a1631d859f78_Meta);
            finder.RefDic.Add("2dfe0c27-6bca-4af7-ba6b-a1631d859f78.Decorators", temp_2dfe0c27_6bca_4af7_ba6b_a1631d859f78_Decorators);
            finder.RefDic.Add("2dfe0c27-6bca-4af7-ba6b-a1631d859f78.WaitTime", temp_2dfe0c27_6bca_4af7_ba6b_a1631d859f78_WaitTime);
            finder.RefDic.Add("2f7d3ba7-cfbb-416a-a003-e1d3ed6f0707", node_2f7d3ba7_cfbb_416a_a003_e1d3ed6f0707);
            finder.RefDic.Add("2f7d3ba7-cfbb-416a-a003-e1d3ed6f0707.Meta", temp_2f7d3ba7_cfbb_416a_a003_e1d3ed6f0707_Meta);
            finder.RefDic.Add("2f7d3ba7-cfbb-416a-a003-e1d3ed6f0707.Decorators", temp_2f7d3ba7_cfbb_416a_a003_e1d3ed6f0707_Decorators);
            finder.RefDic.Add("2f7d3ba7-cfbb-416a-a003-e1d3ed6f0707.Info", temp_2f7d3ba7_cfbb_416a_a003_e1d3ed6f0707_Info);
            finder.RefDic.Add("2f7d3ba7-cfbb-416a-a003-e1d3ed6f0707.Info.Text", temp_2f7d3ba7_cfbb_416a_a003_e1d3ed6f0707_Info_Text);
            finder.RefDic.Add("2f7d3ba7-cfbb-416a-a003-e1d3ed6f0707.Info.Ref_Transform", temp_2f7d3ba7_cfbb_416a_a003_e1d3ed6f0707_Info_Ref_Transform);
            finder.RefDic.Add("2f7d3ba7-cfbb-416a-a003-e1d3ed6f0707.Info.Ref_GameObject", temp_2f7d3ba7_cfbb_416a_a003_e1d3ed6f0707_Info_Ref_GameObject);
            finder.RefDic.Add("46091aac-2293-44bf-881e-5e3476886543", node_46091aac_2293_44bf_881e_5e3476886543);
            finder.RefDic.Add("46091aac-2293-44bf-881e-5e3476886543.Meta", temp_46091aac_2293_44bf_881e_5e3476886543_Meta);
            finder.RefDic.Add("46091aac-2293-44bf-881e-5e3476886543.Decorators", temp_46091aac_2293_44bf_881e_5e3476886543_Decorators);
            finder.RefDic.Add("46091aac-2293-44bf-881e-5e3476886543.Children", temp_46091aac_2293_44bf_881e_5e3476886543_Children);
            finder.RefDic.Add("46091aac-2293-44bf-881e-5e3476886543.WaitTime", temp_46091aac_2293_44bf_881e_5e3476886543_WaitTime);
            finder.RefDic.Add("62f41474-7568-45c7-946e-cdf5ad45fff4", node_62f41474_7568_45c7_946e_cdf5ad45fff4);
            finder.RefDic.Add("62f41474-7568-45c7-946e-cdf5ad45fff4.Meta", temp_62f41474_7568_45c7_946e_cdf5ad45fff4_Meta);
            finder.RefDic.Add("62f41474-7568-45c7-946e-cdf5ad45fff4.Decorators", temp_62f41474_7568_45c7_946e_cdf5ad45fff4_Decorators);
            finder.RefDic.Add("62f41474-7568-45c7-946e-cdf5ad45fff4.WaitTime", temp_62f41474_7568_45c7_946e_cdf5ad45fff4_WaitTime);
            finder.RefDic.Add("a734621c-96b6-4a26-898c-0e4d45f5dba2", node_a734621c_96b6_4a26_898c_0e4d45f5dba2);
            finder.RefDic.Add("a734621c-96b6-4a26-898c-0e4d45f5dba2.Meta", temp_a734621c_96b6_4a26_898c_0e4d45f5dba2_Meta);
            finder.RefDic.Add("a734621c-96b6-4a26-898c-0e4d45f5dba2.Decorators", temp_a734621c_96b6_4a26_898c_0e4d45f5dba2_Decorators);
            finder.RefDic.Add("a734621c-96b6-4a26-898c-0e4d45f5dba2.Children", temp_a734621c_96b6_4a26_898c_0e4d45f5dba2_Children);
            finder.RefDic.Add("f215473e-1bd7-451b-9d38-79a052d9b203", deco_f215473e_1bd7_451b_9d38_79a052d9b203);
            //添加实例到引用查找器 25

            //添加树实例到引用查找器
            finder.RefDic.Add("71ad0dfc-da1a-4b3c-b95d-b4276312641a", tree);

            #region 初始化成员值

            //初始化成员值

            //因为引用类型会使用值类型。所以优先初始化值类型，后生成引用类型。
            //优先初始化内层实例，然后初始化外层实例。

            //2f7d3ba7-cfbb-416a-a003-e1d3ed6f0707.Info.Text.value
            temp_2f7d3ba7_cfbb_416a_a003_e1d3ed6f0707_Info_Text.value = "Hello world!";



            //2dfe0c27-6bca-4af7-ba6b-a1631d859f78.Meta.x
            temp_2dfe0c27_6bca_4af7_ba6b_a1631d859f78_Meta.x = 237.0001f;
            //2dfe0c27-6bca-4af7-ba6b-a1631d859f78.Meta.y
            temp_2dfe0c27_6bca_4af7_ba6b_a1631d859f78_Meta.y = 470.7251f;


            //2dfe0c27-6bca-4af7-ba6b-a1631d859f78.WaitTime.value
            temp_2dfe0c27_6bca_4af7_ba6b_a1631d859f78_WaitTime.value = 5f;

            //2f7d3ba7-cfbb-416a-a003-e1d3ed6f0707.Meta.x
            temp_2f7d3ba7_cfbb_416a_a003_e1d3ed6f0707_Meta.x = 440f;
            //2f7d3ba7-cfbb-416a-a003-e1d3ed6f0707.Meta.y
            temp_2f7d3ba7_cfbb_416a_a003_e1d3ed6f0707_Meta.y = 312f;
            //2f7d3ba7-cfbb-416a-a003-e1d3ed6f0707.Meta.Name
            temp_2f7d3ba7_cfbb_416a_a003_e1d3ed6f0707_Meta.Name = "";
            //2f7d3ba7-cfbb-416a-a003-e1d3ed6f0707.Meta.FriendlyName
            temp_2f7d3ba7_cfbb_416a_a003_e1d3ed6f0707_Meta.FriendlyName = "";
            //2f7d3ba7-cfbb-416a-a003-e1d3ed6f0707.Meta.Description
            temp_2f7d3ba7_cfbb_416a_a003_e1d3ed6f0707_Meta.Description = "test";
            //2f7d3ba7-cfbb-416a-a003-e1d3ed6f0707.Meta.FriendlyDescription
            temp_2f7d3ba7_cfbb_416a_a003_e1d3ed6f0707_Meta.FriendlyDescription = "";
            //2f7d3ba7-cfbb-416a-a003-e1d3ed6f0707.Meta.Comment
            temp_2f7d3ba7_cfbb_416a_a003_e1d3ed6f0707_Meta.Comment = "";


            //2f7d3ba7-cfbb-416a-a003-e1d3ed6f0707.Info.Text
            if (finder.TryGetRefValue<Megumin.Binding.RefVar_String>(
                "2f7d3ba7-cfbb-416a-a003-e1d3ed6f0707.Info.Text",
                out var temp_ref_2f7d3ba7_cfbb_416a_a003_e1d3ed6f0707_Info_Text))
            {
                temp_2f7d3ba7_cfbb_416a_a003_e1d3ed6f0707_Info.Text = temp_ref_2f7d3ba7_cfbb_416a_a003_e1d3ed6f0707_Info_Text;
            }

            //2f7d3ba7-cfbb-416a-a003-e1d3ed6f0707.Info.Ref_Transform
            if (finder.TryGetRefValue<Megumin.Binding.RefVar_Transform>(
                "2f7d3ba7-cfbb-416a-a003-e1d3ed6f0707.Info.Ref_Transform",
                out var temp_ref_2f7d3ba7_cfbb_416a_a003_e1d3ed6f0707_Info_Ref_Transform))
            {
                temp_2f7d3ba7_cfbb_416a_a003_e1d3ed6f0707_Info.Ref_Transform = temp_ref_2f7d3ba7_cfbb_416a_a003_e1d3ed6f0707_Info_Ref_Transform;
            }

            //2f7d3ba7-cfbb-416a-a003-e1d3ed6f0707.Info.Ref_GameObject
            if (finder.TryGetRefValue<Megumin.Binding.RefVar_GameObject>(
                "2f7d3ba7-cfbb-416a-a003-e1d3ed6f0707.Info.Ref_GameObject",
                out var temp_ref_2f7d3ba7_cfbb_416a_a003_e1d3ed6f0707_Info_Ref_GameObject))
            {
                temp_2f7d3ba7_cfbb_416a_a003_e1d3ed6f0707_Info.Ref_GameObject = temp_ref_2f7d3ba7_cfbb_416a_a003_e1d3ed6f0707_Info_Ref_GameObject;
            }


            //46091aac-2293-44bf-881e-5e3476886543.Meta.x
            temp_46091aac_2293_44bf_881e_5e3476886543_Meta.x = 237f;
            //46091aac-2293-44bf-881e-5e3476886543.Meta.y
            temp_46091aac_2293_44bf_881e_5e3476886543_Meta.y = 312f;
            //46091aac-2293-44bf-881e-5e3476886543.Meta.Name
            temp_46091aac_2293_44bf_881e_5e3476886543_Meta.Name = "";
            //46091aac-2293-44bf-881e-5e3476886543.Meta.FriendlyName
            temp_46091aac_2293_44bf_881e_5e3476886543_Meta.FriendlyName = "";
            //46091aac-2293-44bf-881e-5e3476886543.Meta.Description
            temp_46091aac_2293_44bf_881e_5e3476886543_Meta.Description = "";
            //46091aac-2293-44bf-881e-5e3476886543.Meta.FriendlyDescription
            temp_46091aac_2293_44bf_881e_5e3476886543_Meta.FriendlyDescription = "";
            //46091aac-2293-44bf-881e-5e3476886543.Meta.Comment
            temp_46091aac_2293_44bf_881e_5e3476886543_Meta.Comment = "";


            //46091aac-2293-44bf-881e-5e3476886543.Children.0
            if (finder.TryGetRefValue<Megumin.AI.BehaviorTree.Wait>(
                "2dfe0c27-6bca-4af7-ba6b-a1631d859f78",
                out var temp_ref_46091aac_2293_44bf_881e_5e3476886543_Children_0))
            {
                temp_46091aac_2293_44bf_881e_5e3476886543_Children.Insert(0, temp_ref_46091aac_2293_44bf_881e_5e3476886543_Children_0);
            }


            //46091aac-2293-44bf-881e-5e3476886543.WaitTime.value
            temp_46091aac_2293_44bf_881e_5e3476886543_WaitTime.value = 5f;

            //62f41474-7568-45c7-946e-cdf5ad45fff4.Meta.x
            temp_62f41474_7568_45c7_946e_cdf5ad45fff4_Meta.x = 31f;
            //62f41474-7568-45c7-946e-cdf5ad45fff4.Meta.y
            temp_62f41474_7568_45c7_946e_cdf5ad45fff4_Meta.y = 312f;
            //62f41474-7568-45c7-946e-cdf5ad45fff4.Meta.Name
            temp_62f41474_7568_45c7_946e_cdf5ad45fff4_Meta.Name = "";
            //62f41474-7568-45c7-946e-cdf5ad45fff4.Meta.FriendlyName
            temp_62f41474_7568_45c7_946e_cdf5ad45fff4_Meta.FriendlyName = "";
            //62f41474-7568-45c7-946e-cdf5ad45fff4.Meta.Description
            temp_62f41474_7568_45c7_946e_cdf5ad45fff4_Meta.Description = "";
            //62f41474-7568-45c7-946e-cdf5ad45fff4.Meta.FriendlyDescription
            temp_62f41474_7568_45c7_946e_cdf5ad45fff4_Meta.FriendlyDescription = "";
            //62f41474-7568-45c7-946e-cdf5ad45fff4.Meta.Comment
            temp_62f41474_7568_45c7_946e_cdf5ad45fff4_Meta.Comment = "";


            //62f41474-7568-45c7-946e-cdf5ad45fff4.WaitTime.value
            temp_62f41474_7568_45c7_946e_cdf5ad45fff4_WaitTime.value = 5f;

            //a734621c-96b6-4a26-898c-0e4d45f5dba2.Meta.x
            temp_a734621c_96b6_4a26_898c_0e4d45f5dba2_Meta.x = 225.3119f;
            //a734621c-96b6-4a26-898c-0e4d45f5dba2.Meta.y
            temp_a734621c_96b6_4a26_898c_0e4d45f5dba2_Meta.y = 86.59545f;
            //a734621c-96b6-4a26-898c-0e4d45f5dba2.Meta.Name
            temp_a734621c_96b6_4a26_898c_0e4d45f5dba2_Meta.Name = "";
            //a734621c-96b6-4a26-898c-0e4d45f5dba2.Meta.FriendlyName
            temp_a734621c_96b6_4a26_898c_0e4d45f5dba2_Meta.FriendlyName = "";
            //a734621c-96b6-4a26-898c-0e4d45f5dba2.Meta.Description
            temp_a734621c_96b6_4a26_898c_0e4d45f5dba2_Meta.Description = "";
            //a734621c-96b6-4a26-898c-0e4d45f5dba2.Meta.FriendlyDescription
            temp_a734621c_96b6_4a26_898c_0e4d45f5dba2_Meta.FriendlyDescription = "";
            //a734621c-96b6-4a26-898c-0e4d45f5dba2.Meta.Comment
            temp_a734621c_96b6_4a26_898c_0e4d45f5dba2_Meta.Comment = "";
            //a734621c-96b6-4a26-898c-0e4d45f5dba2.Meta.IsStartNode
            temp_a734621c_96b6_4a26_898c_0e4d45f5dba2_Meta.IsStartNode = true;

            //a734621c-96b6-4a26-898c-0e4d45f5dba2.Decorators.0
            if (finder.TryGetRefValue<Megumin.AI.BehaviorTree.Loop_Decorator>(
                "f215473e-1bd7-451b-9d38-79a052d9b203",
                out var temp_ref_a734621c_96b6_4a26_898c_0e4d45f5dba2_Decorators_0))
            {
                temp_a734621c_96b6_4a26_898c_0e4d45f5dba2_Decorators.Insert(0, temp_ref_a734621c_96b6_4a26_898c_0e4d45f5dba2_Decorators_0);
            }


            //a734621c-96b6-4a26-898c-0e4d45f5dba2.Children.0
            if (finder.TryGetRefValue<Megumin.AI.BehaviorTree.Wait>(
                "62f41474-7568-45c7-946e-cdf5ad45fff4",
                out var temp_ref_a734621c_96b6_4a26_898c_0e4d45f5dba2_Children_0))
            {
                temp_a734621c_96b6_4a26_898c_0e4d45f5dba2_Children.Insert(0, temp_ref_a734621c_96b6_4a26_898c_0e4d45f5dba2_Children_0);
            }

            //a734621c-96b6-4a26-898c-0e4d45f5dba2.Children.1
            if (finder.TryGetRefValue<Megumin.AI.BehaviorTree.WaitDo>(
                "46091aac-2293-44bf-881e-5e3476886543",
                out var temp_ref_a734621c_96b6_4a26_898c_0e4d45f5dba2_Children_1))
            {
                temp_a734621c_96b6_4a26_898c_0e4d45f5dba2_Children.Insert(1, temp_ref_a734621c_96b6_4a26_898c_0e4d45f5dba2_Children_1);
            }

            //a734621c-96b6-4a26-898c-0e4d45f5dba2.Children.2
            if (finder.TryGetRefValue<Megumin.AI.BehaviorTree.Log>(
                "2f7d3ba7-cfbb-416a-a003-e1d3ed6f0707",
                out var temp_ref_a734621c_96b6_4a26_898c_0e4d45f5dba2_Children_2))
            {
                temp_a734621c_96b6_4a26_898c_0e4d45f5dba2_Children.Insert(2, temp_ref_a734621c_96b6_4a26_898c_0e4d45f5dba2_Children_2);
            }


            //2dfe0c27-6bca-4af7-ba6b-a1631d859f78.Meta
            if (finder.TryGetRefValue<Megumin.AI.BehaviorTree.NodeMeta>(
                "2dfe0c27-6bca-4af7-ba6b-a1631d859f78.Meta",
                out var temp_ref_2dfe0c27_6bca_4af7_ba6b_a1631d859f78_Meta))
            {
                node_2dfe0c27_6bca_4af7_ba6b_a1631d859f78.Meta = temp_ref_2dfe0c27_6bca_4af7_ba6b_a1631d859f78_Meta;
            }

            //2dfe0c27-6bca-4af7-ba6b-a1631d859f78.Decorators
            if (finder.TryGetRefValue<System.Collections.Generic.List<Megumin.AI.BehaviorTree.IDecorator>>(
                "2dfe0c27-6bca-4af7-ba6b-a1631d859f78.Decorators",
                out var temp_ref_2dfe0c27_6bca_4af7_ba6b_a1631d859f78_Decorators))
            {
                node_2dfe0c27_6bca_4af7_ba6b_a1631d859f78.Decorators = temp_ref_2dfe0c27_6bca_4af7_ba6b_a1631d859f78_Decorators;
            }

            //2dfe0c27-6bca-4af7-ba6b-a1631d859f78.WaitTime
            if (finder.TryGetRefValue<Megumin.Binding.RefVar_Float>(
                "2dfe0c27-6bca-4af7-ba6b-a1631d859f78.WaitTime",
                out var temp_ref_2dfe0c27_6bca_4af7_ba6b_a1631d859f78_WaitTime))
            {
                node_2dfe0c27_6bca_4af7_ba6b_a1631d859f78.WaitTime = temp_ref_2dfe0c27_6bca_4af7_ba6b_a1631d859f78_WaitTime;
            }

            //2dfe0c27-6bca-4af7-ba6b-a1631d859f78.GUID
            node_2dfe0c27_6bca_4af7_ba6b_a1631d859f78.GUID = "2dfe0c27-6bca-4af7-ba6b-a1631d859f78";

            //2f7d3ba7-cfbb-416a-a003-e1d3ed6f0707.Meta
            if (finder.TryGetRefValue<Megumin.AI.BehaviorTree.NodeMeta>(
                "2f7d3ba7-cfbb-416a-a003-e1d3ed6f0707.Meta",
                out var temp_ref_2f7d3ba7_cfbb_416a_a003_e1d3ed6f0707_Meta))
            {
                node_2f7d3ba7_cfbb_416a_a003_e1d3ed6f0707.Meta = temp_ref_2f7d3ba7_cfbb_416a_a003_e1d3ed6f0707_Meta;
            }

            //2f7d3ba7-cfbb-416a-a003-e1d3ed6f0707.Decorators
            if (finder.TryGetRefValue<System.Collections.Generic.List<Megumin.AI.BehaviorTree.IDecorator>>(
                "2f7d3ba7-cfbb-416a-a003-e1d3ed6f0707.Decorators",
                out var temp_ref_2f7d3ba7_cfbb_416a_a003_e1d3ed6f0707_Decorators))
            {
                node_2f7d3ba7_cfbb_416a_a003_e1d3ed6f0707.Decorators = temp_ref_2f7d3ba7_cfbb_416a_a003_e1d3ed6f0707_Decorators;
            }

            //2f7d3ba7-cfbb-416a-a003-e1d3ed6f0707.Info
            if (finder.TryGetRefValue<Megumin.AI.LogInfo>(
                "2f7d3ba7-cfbb-416a-a003-e1d3ed6f0707.Info",
                out var temp_ref_2f7d3ba7_cfbb_416a_a003_e1d3ed6f0707_Info))
            {
                node_2f7d3ba7_cfbb_416a_a003_e1d3ed6f0707.Info = temp_ref_2f7d3ba7_cfbb_416a_a003_e1d3ed6f0707_Info;
            }

            //2f7d3ba7-cfbb-416a-a003-e1d3ed6f0707.GUID
            node_2f7d3ba7_cfbb_416a_a003_e1d3ed6f0707.GUID = "2f7d3ba7-cfbb-416a-a003-e1d3ed6f0707";

            //46091aac-2293-44bf-881e-5e3476886543.Meta
            if (finder.TryGetRefValue<Megumin.AI.BehaviorTree.NodeMeta>(
                "46091aac-2293-44bf-881e-5e3476886543.Meta",
                out var temp_ref_46091aac_2293_44bf_881e_5e3476886543_Meta))
            {
                node_46091aac_2293_44bf_881e_5e3476886543.Meta = temp_ref_46091aac_2293_44bf_881e_5e3476886543_Meta;
            }

            //46091aac-2293-44bf-881e-5e3476886543.Decorators
            if (finder.TryGetRefValue<System.Collections.Generic.List<Megumin.AI.BehaviorTree.IDecorator>>(
                "46091aac-2293-44bf-881e-5e3476886543.Decorators",
                out var temp_ref_46091aac_2293_44bf_881e_5e3476886543_Decorators))
            {
                node_46091aac_2293_44bf_881e_5e3476886543.Decorators = temp_ref_46091aac_2293_44bf_881e_5e3476886543_Decorators;
            }

            //46091aac-2293-44bf-881e-5e3476886543.Children
            if (finder.TryGetRefValue<System.Collections.Generic.List<Megumin.AI.BehaviorTree.BTNode>>(
                "46091aac-2293-44bf-881e-5e3476886543.Children",
                out var temp_ref_46091aac_2293_44bf_881e_5e3476886543_Children))
            {
                node_46091aac_2293_44bf_881e_5e3476886543.Children = temp_ref_46091aac_2293_44bf_881e_5e3476886543_Children;
            }

            //46091aac-2293-44bf-881e-5e3476886543.WaitTime
            if (finder.TryGetRefValue<Megumin.Binding.RefVar_Float>(
                "46091aac-2293-44bf-881e-5e3476886543.WaitTime",
                out var temp_ref_46091aac_2293_44bf_881e_5e3476886543_WaitTime))
            {
                node_46091aac_2293_44bf_881e_5e3476886543.WaitTime = temp_ref_46091aac_2293_44bf_881e_5e3476886543_WaitTime;
            }

            //46091aac-2293-44bf-881e-5e3476886543.GUID
            node_46091aac_2293_44bf_881e_5e3476886543.GUID = "46091aac-2293-44bf-881e-5e3476886543";

            //62f41474-7568-45c7-946e-cdf5ad45fff4.Meta
            if (finder.TryGetRefValue<Megumin.AI.BehaviorTree.NodeMeta>(
                "62f41474-7568-45c7-946e-cdf5ad45fff4.Meta",
                out var temp_ref_62f41474_7568_45c7_946e_cdf5ad45fff4_Meta))
            {
                node_62f41474_7568_45c7_946e_cdf5ad45fff4.Meta = temp_ref_62f41474_7568_45c7_946e_cdf5ad45fff4_Meta;
            }

            //62f41474-7568-45c7-946e-cdf5ad45fff4.Decorators
            if (finder.TryGetRefValue<System.Collections.Generic.List<Megumin.AI.BehaviorTree.IDecorator>>(
                "62f41474-7568-45c7-946e-cdf5ad45fff4.Decorators",
                out var temp_ref_62f41474_7568_45c7_946e_cdf5ad45fff4_Decorators))
            {
                node_62f41474_7568_45c7_946e_cdf5ad45fff4.Decorators = temp_ref_62f41474_7568_45c7_946e_cdf5ad45fff4_Decorators;
            }

            //62f41474-7568-45c7-946e-cdf5ad45fff4.WaitTime
            if (finder.TryGetRefValue<Megumin.Binding.RefVar_Float>(
                "62f41474-7568-45c7-946e-cdf5ad45fff4.WaitTime",
                out var temp_ref_62f41474_7568_45c7_946e_cdf5ad45fff4_WaitTime))
            {
                node_62f41474_7568_45c7_946e_cdf5ad45fff4.WaitTime = temp_ref_62f41474_7568_45c7_946e_cdf5ad45fff4_WaitTime;
            }

            //62f41474-7568-45c7-946e-cdf5ad45fff4.GUID
            node_62f41474_7568_45c7_946e_cdf5ad45fff4.GUID = "62f41474-7568-45c7-946e-cdf5ad45fff4";

            //a734621c-96b6-4a26-898c-0e4d45f5dba2.Meta
            if (finder.TryGetRefValue<Megumin.AI.BehaviorTree.NodeMeta>(
                "a734621c-96b6-4a26-898c-0e4d45f5dba2.Meta",
                out var temp_ref_a734621c_96b6_4a26_898c_0e4d45f5dba2_Meta))
            {
                node_a734621c_96b6_4a26_898c_0e4d45f5dba2.Meta = temp_ref_a734621c_96b6_4a26_898c_0e4d45f5dba2_Meta;
            }

            //a734621c-96b6-4a26-898c-0e4d45f5dba2.Decorators
            if (finder.TryGetRefValue<System.Collections.Generic.List<Megumin.AI.BehaviorTree.IDecorator>>(
                "a734621c-96b6-4a26-898c-0e4d45f5dba2.Decorators",
                out var temp_ref_a734621c_96b6_4a26_898c_0e4d45f5dba2_Decorators))
            {
                node_a734621c_96b6_4a26_898c_0e4d45f5dba2.Decorators = temp_ref_a734621c_96b6_4a26_898c_0e4d45f5dba2_Decorators;
            }

            //a734621c-96b6-4a26-898c-0e4d45f5dba2.Children
            if (finder.TryGetRefValue<System.Collections.Generic.List<Megumin.AI.BehaviorTree.BTNode>>(
                "a734621c-96b6-4a26-898c-0e4d45f5dba2.Children",
                out var temp_ref_a734621c_96b6_4a26_898c_0e4d45f5dba2_Children))
            {
                node_a734621c_96b6_4a26_898c_0e4d45f5dba2.Children = temp_ref_a734621c_96b6_4a26_898c_0e4d45f5dba2_Children;
            }

            //a734621c-96b6-4a26-898c-0e4d45f5dba2.GUID
            node_a734621c_96b6_4a26_898c_0e4d45f5dba2.GUID = "a734621c-96b6-4a26-898c-0e4d45f5dba2";

            //f215473e-1bd7-451b-9d38-79a052d9b203.GUID
            deco_f215473e_1bd7_451b_9d38_79a052d9b203.GUID = "f215473e-1bd7-451b-9d38-79a052d9b203";

            #endregion

            #region 添加实例到树

            //添加参数
            //以上添加到树 0 参数实例

            //添加普通对象
            tree.InitAddObjNotTreeElement(temp_2f7d3ba7_cfbb_416a_a003_e1d3ed6f0707_Info_Text);
            tree.InitAddObjNotTreeElement(temp_2f7d3ba7_cfbb_416a_a003_e1d3ed6f0707_Info_Ref_Transform);
            tree.InitAddObjNotTreeElement(temp_2f7d3ba7_cfbb_416a_a003_e1d3ed6f0707_Info_Ref_GameObject);
            tree.InitAddObjNotTreeElement(temp_2dfe0c27_6bca_4af7_ba6b_a1631d859f78_Meta);
            tree.InitAddObjNotTreeElement(temp_2dfe0c27_6bca_4af7_ba6b_a1631d859f78_Decorators);
            tree.InitAddObjNotTreeElement(temp_2dfe0c27_6bca_4af7_ba6b_a1631d859f78_WaitTime);
            tree.InitAddObjNotTreeElement(temp_2f7d3ba7_cfbb_416a_a003_e1d3ed6f0707_Meta);
            tree.InitAddObjNotTreeElement(temp_2f7d3ba7_cfbb_416a_a003_e1d3ed6f0707_Decorators);
            tree.InitAddObjNotTreeElement(temp_2f7d3ba7_cfbb_416a_a003_e1d3ed6f0707_Info);
            tree.InitAddObjNotTreeElement(temp_46091aac_2293_44bf_881e_5e3476886543_Meta);
            tree.InitAddObjNotTreeElement(temp_46091aac_2293_44bf_881e_5e3476886543_Decorators);
            tree.InitAddObjNotTreeElement(temp_46091aac_2293_44bf_881e_5e3476886543_Children);
            tree.InitAddObjNotTreeElement(temp_46091aac_2293_44bf_881e_5e3476886543_WaitTime);
            tree.InitAddObjNotTreeElement(temp_62f41474_7568_45c7_946e_cdf5ad45fff4_Meta);
            tree.InitAddObjNotTreeElement(temp_62f41474_7568_45c7_946e_cdf5ad45fff4_Decorators);
            tree.InitAddObjNotTreeElement(temp_62f41474_7568_45c7_946e_cdf5ad45fff4_WaitTime);
            tree.InitAddObjNotTreeElement(temp_a734621c_96b6_4a26_898c_0e4d45f5dba2_Meta);
            tree.InitAddObjNotTreeElement(temp_a734621c_96b6_4a26_898c_0e4d45f5dba2_Decorators);
            tree.InitAddObjNotTreeElement(temp_a734621c_96b6_4a26_898c_0e4d45f5dba2_Children);
            //以上添加到树 19 普通对象

            //添加装饰器
            tree.InitAddObjTreeElement(deco_f215473e_1bd7_451b_9d38_79a052d9b203);
            //以上添加到树 1 装饰器

            //添加节点
            tree.InitAddObjTreeElement(node_2dfe0c27_6bca_4af7_ba6b_a1631d859f78);
            tree.InitAddObjTreeElement(node_2f7d3ba7_cfbb_416a_a003_e1d3ed6f0707);
            tree.InitAddObjTreeElement(node_46091aac_2293_44bf_881e_5e3476886543);
            tree.InitAddObjTreeElement(node_62f41474_7568_45c7_946e_cdf5ad45fff4);
            tree.InitAddObjTreeElement(node_a734621c_96b6_4a26_898c_0e4d45f5dba2);
            //以上添加到树 5 节点

            #endregion

            #region 设置开始节点 和 装饰器Owner

            tree.StartNode = node_a734621c_96b6_4a26_898c_0e4d45f5dba2;
            deco_f215473e_1bd7_451b_9d38_79a052d9b203.Owner = node_a734621c_96b6_4a26_898c_0e4d45f5dba2;

            #endregion

            tree.UpdateNodeIndexDepth();

            PostInit(initOption, tree);

            return tree;
        }
    }
}

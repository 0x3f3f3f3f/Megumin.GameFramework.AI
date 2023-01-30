using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Megumin.GameFramework.AI.BehaviorTree
{
    public class BehaviorTreeAsset : ScriptableObject
    {
        public string test = "aaa";
        public List<BehaviorTreeNodeAsset> Nodes = new List<BehaviorTreeNodeAsset>();
        public BehaviorTreeNodeAsset StartNode;

        public BehaviorTree CreateTree()
        {
            var tree = new BehaviorTree();
            tree.Asset = this;
            LoadLast(tree);
            return tree;
        }

        private void LoadLast(BehaviorTree tree)
        {
            var wait = new Wait();
            var log = new Log();
            var seq = new Sequence();
            seq.children.Add(wait);
            seq.children.Add(log);

            var loop = new Loop();
            seq.Derators = new object[] { loop };
            //var loop = new Repeater();
            //loop.child = seq;

            var check = new CheckBool();
            var remap = new Remap();
            log.Derators = new object[] { check, remap };
            tree.StartNode = seq;
        }

        private void Load4(BehaviorTree tree)
        {
            var wait = new Wait();
            var log = new Log();
            var seq = new Sequence();
            seq.children.Add(wait);
            seq.children.Add(log);

            var loop = new Repeater();
            loop.child = seq;

            var check = new CheckBool();
            var remap = new Remap();
            log.Derators = new object[] { check, remap };
            tree.StartNode = loop;
        }

        private void Load3(BehaviorTree tree)
        {
            var wait = new Wait();
            var log = new Log();
            var seq = new Sequence();
            seq.children.Add(wait);
            seq.children.Add(log);

            var loop = new Repeater();
            loop.child = seq;

            var check = new CheckBool();
            log.Derators = new object[] { check };
            tree.StartNode = loop;
        }

        private void Load2(BehaviorTree tree)
        {
            var wait = new Wait();
            var log = new Log();
            var seq = new Sequence();
            seq.children.Add(wait);
            seq.children.Add(log);

            var loop = new Repeater();
            loop.child = seq;

            tree.StartNode = loop;
        }

        private void Load1(BehaviorTree tree)
        {
            var wait = new Wait();
            var log = new Log();
            var seq = new Sequence();
            seq.children.Add(wait);
            seq.children.Add(log);

            tree.StartNode = seq;
        }
    }
}

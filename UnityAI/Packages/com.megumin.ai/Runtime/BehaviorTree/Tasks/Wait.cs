using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Megumin.GameFramework.AI.Serialization;
using UnityEngine;

namespace Megumin.GameFramework.AI.BehaviorTree
{
    [Category("CategoryTest")]
    [Icon("ICONS/sg_graph_icon.png")]
    public class Wait : ActionTaskNode, IParameterDataSerializationCallbackReceiver
    {
        public float waitTime = 3f;
        public GameObject TestRef;
        public List<GameObject> TestList;
        public List<int> TestList2;
        public string TestUser;

        float entertime;
        protected override void OnEnter()
        {
            entertime = Time.time;
        }

        protected override Status OnTick()
        {
            //Debug.Log($"Wait Time :{Time.time - entertime}");
            if (Time.time - entertime > waitTime)
            {
                return Status.Succeeded;
            }
            return Status.Running;
        }

        public void OnAfterDeserialize(List<CustomParameterData> source)
        {
            foreach (var item in source)
            {
                if (item.MemberName == nameof(TestUser))
                {
                    TestUser = item.Value;
                }
            }
        }

        public void OnBeforeSerialize(List<CustomParameterData> desitination, List<string> ignoreMemberOnSerialize)
        {
            ignoreMemberOnSerialize.Add(nameof(TestUser));
            desitination.Add(new CustomParameterData()
            {
                MemberName = nameof(TestUser),
                Value = TestUser,
            });
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Megumin.Binding.Test
{
    public class BindingsSO : ScriptableObject
    {
        public BindingVar<int> BindInt
            = new BindableValueInt() { BindingPath = "UnityEngine.GameObject/layer" };

        public BindingVar<int> NeedOverrideInt1
            = new() { BindingPath = "UnityEngine.Time/captureFramerate" };

        public BindingVar<int> NeedOverrideInt2
            = new() { BindingPath = "UnityEngine.SceneManagement.SceneManager/sceneCountInBuildSettings" };

        public BindingVar<int> NeedOverrideInt3
            = new() { BindingPath = "UnityEngine.Application/targetFrameRate" };
    }
}

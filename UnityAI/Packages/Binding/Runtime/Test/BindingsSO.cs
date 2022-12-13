using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Megumin.Binding.Test
{
    public class BindingsSO : ScriptableObject
    {
        public BindableValue<int> BindInt
            = new BindableValueInt() { BindingPath = "UnityEngine.GameObject/layer" };

        public BindableValue<int> NeedOverrideInt1
            = new BindableValue<int>() { BindingPath = "UnityEngine.Time/captureFramerate" };

        public BindableValue<int> NeedOverrideInt2
            = new BindableValue<int>() { BindingPath = "UnityEngine.SceneManagement.SceneManager/sceneCountInBuildSettings" };

        public BindableValue<int> NeedOverrideInt3
            = new BindableValue<int>() { BindingPath = "UnityEngine.Application/targetFrameRate" };
    }
}

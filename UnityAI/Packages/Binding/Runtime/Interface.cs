using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;
using System.Linq;

namespace Megumin.Binding
{
    public interface IData
    {

    }

    public interface IData<T> : IData
    {
        T Value { get; set; }
    }

    public interface IBindable
    {
        //string BindingPath { get; set; }
    }

    public interface IBindable<T> : IBindable
    {
        //T DefaultValue { get; set; }
    }
}

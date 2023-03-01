using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Megumin.GameFramework.AI
{
    [AttributeUsage(AttributeTargets.All)]
    public class ColorAttribute : Attribute
    {
        public Color Color { get; set; }
        public float r { get; set; }
        public float g { get; set; }
        public float b { get; set; }
        public float a { get; set; } = 1f;

        public ColorAttribute(float r, float g, float b, float a = 1f)
        {
            Color = new Color(r, g, b, a);
            this.r = r;
            this.g = g;
            this.b = b;
            this.a = a;
        }
    }
}

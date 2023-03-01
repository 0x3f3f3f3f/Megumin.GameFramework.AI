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
        public double r { get; set; }
        public double g { get; set; }
        public double b { get; set; }
        public double a { get; set; } = 1f;

        public ColorAttribute(double r, double g, double b, double a = 1f)
        {
            Color = new Color((float)r, (float)g, (float)b, (float)a);
            this.r = r;
            this.g = g;
            this.b = b;
            this.a = a;
        }
    }
}

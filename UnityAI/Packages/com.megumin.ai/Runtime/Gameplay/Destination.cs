using System;
using System.Collections;
using System.Collections.Generic;
using Megumin.Binding;
using UnityEngine;

namespace Megumin.AI
{
    public enum DestinationType
    {
        /// <summary>
        /// If Transform is null, return Vector
        /// </summary>
        Auto,
        Vector,
        Transform,
    }

    [Serializable]
    public class Destination
    {
        public DestinationType Type = DestinationType.Auto;
        [Space]
        public RefVar_Vector3 Dest_Vector;
        public RefVar_Transform Dest_Transform;

        public Vector3 GetDestination()
        {
            switch (Type)
            {
                case DestinationType.Auto:
                    break;
                case DestinationType.Vector:
                    return Dest_Vector;
                case DestinationType.Transform:
                    return Dest_Transform.Value.position;
                default:
                    break;
            }

            if (Dest_Transform?.Value)
            {
                return Dest_Transform.Value.position;
            }
            return Dest_Vector;
        }
    }
}



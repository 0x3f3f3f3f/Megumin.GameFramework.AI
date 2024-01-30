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
        GameObject,
        Transform,
    }

    [Serializable]
    public class Destination
    {
        public DestinationType Type = DestinationType.Auto;
        [Space]
        public RefVar_Vector3 Dest_Vector;
        public RefVar_GameObject Dest_GameObject;
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
                case DestinationType.GameObject:
                    return Dest_GameObject.Value.transform.position;
                default:
                    break;
            }

            if (Dest_GameObject?.Value)
            {
                return Dest_GameObject.Value.transform.position;
            }

            if (Dest_Transform?.Value)
            {
                return Dest_Transform.Value.position;
            }

            return Dest_Vector;
        }

        public GameObject Target
        {
            get
            {
                switch (Type)
                {
                    case DestinationType.Transform:
                        if (Dest_Transform.Value)
                        {
                            return Dest_Transform.Value.gameObject;
                        }
                        else
                        {
                            return null;
                        }
                    case DestinationType.GameObject:
                        return Dest_GameObject.Value;
                    default:
                        break;
                }

                return null;
            }
        }
    }
}



using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Megumin.AI.BehaviorTree;
using UnityEngine;
using UnityEngine.AI;

public class MoveActor : MonoBehaviour, IMoveToable<Vector3>
{
    NavMeshAgent agent;
    public Transform destinationMarker;
    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }


    public bool MoveTo(Vector3 destination)
    {
        if (destinationMarker)
        {
            destinationMarker.position = destination;
        }

        if (agent)
        {
            return agent.SetDestination(destination);
        }
        return false;
    }

    public Transform testTarget;

    [Editor]
    public void Test()
    {
        if (testTarget)
        {
            MoveTo(testTarget.position);
        }
    }
}

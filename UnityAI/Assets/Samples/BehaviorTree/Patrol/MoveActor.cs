using System.Collections;
using System.Collections.Generic;
using Megumin.AI.BehaviorTree;
using UnityEngine;
using UnityEngine.AI;

public class MoveActor : MonoBehaviour, IMoveToable<Vector3>
{
    NavMeshAgent agent;
    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }


    public bool MoveTo(Vector3 destination)
    {
        if (agent)
        {
            return agent.SetDestination(destination);
        }
        return false;
    }
}

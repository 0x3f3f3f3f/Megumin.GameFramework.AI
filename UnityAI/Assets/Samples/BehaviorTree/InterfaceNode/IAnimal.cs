using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Megumin.AI.BehaviorTree;
using Megumin.AI;

public interface IAnimal
{
    string Name { get; set; }
    int Age { get; }
    void Wow();
    int GetAge();
}

public class AnimalWow : BTActionNode<IAnimal>
{
    protected override Status OnTick(BTNode from, object options = null)
    {
        MyAgent.Wow();
        return Status.Succeeded;
    }
}


using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Megumin.GameFramework.AI.BehaviorTree;
using UnityEngine;

public class Test : MonoBehaviour
{
    [SerializeReference]
    public object[] Decorator = new object[5];
    public BTNode test= new BTNode();
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [Editor]
    public void TestAdd()
    {
        Decorator[0] = new Loop();
        Decorator[1] = new CheckBool();
        Decorator[2] = new Cooldown();
        //Decorator.Add(new Loop());

        test.AddDecorator(new Loop());
        test.AddDecorator(new CheckBool());
        test.AddDecorator(new Cooldown());

    }
}

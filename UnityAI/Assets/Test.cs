using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Megumin.AI;
using Megumin.AI.BehaviorTree;
using UnityEngine;
using Megumin;

public class Test : MonoBehaviour
{
    public GameObjectFilter Filter;
    public TagMask test111;
    [SerializeReference]
    public object[] Decorator = new object[5];
    public BTNode test = new BTNode();
    public FailedCode FailedCode;
    public string code;
    public string code2;
    public TestFailedCode2 FailedCode2;
    public string code3;
    public string code4;
    public string code5;

    [TypeSetter]
    public string TestType;
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
        Decorator[0] = new Loop_Decorator();
        Decorator[1] = new CheckBool_Decorator();
        Decorator[2] = new Cooldown_Decorator();
        //Decorator.Add(new Loop());

        test.AddDecorator(new Loop_Decorator());
        test.AddDecorator(new CheckBool_Decorator());
        test.AddDecorator(new Cooldown_Decorator());

    }

    private void OnValidate()
    {
        code = ((int)FailedCode).ToString();
        code2 = Convert.ToString((int)FailedCode, 2).PadLeft(32, '0');

        code3 = ((int)FailedCode2).ToString();
        code4 = Convert.ToString((int)FailedCode2, 2).PadLeft(32, '0');
        int num = (int)FailedCode2;
        code5 = num >= 0 ? "����" : "����";
    }
}

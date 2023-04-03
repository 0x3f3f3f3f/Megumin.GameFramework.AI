using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Megumin.GameFramework.AI;
using Megumin.GameFramework.AI.BehaviorTree;
using UnityEngine;

public class Test : MonoBehaviour
{
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

    private void OnValidate()
    {
        code = ((int)FailedCode).ToString();
        code2 = Convert.ToString((int)FailedCode, 2).PadLeft(32, '0');

        code3 = ((int)FailedCode2).ToString();
        code4 = Convert.ToString((int)FailedCode2, 2).PadLeft(32, '0');
        int num = (int)FailedCode2;
        code5 = num >= 0 ? "正数" : "负数";
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dog : MonoBehaviour, IAnimal
{
    public float WaitTime = 5f;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Wow()
    {
        Debug.Log("Dog Wow!");
    }

    public string Name { get; } = "Spike";
    public int Age { get; } = 8;
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cat : MonoBehaviour, IAnimal
{
    public float WaitTime = 3f;
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
        Debug.Log("Cat Wow!");
    }

    public string Name { get; set; } = "Tom";
    public int Age { get; } = 5;

    public int GetAge()
    {
        return Age;
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class BarrelType : ScriptableObject
{
    [Range(1f, 8f)]
    public float radius = 1;
    public float damage = 10;
    public Color color = Color.red;

    
    // Example just for serializable classes below
    public List<MyClass> thing = new List<MyClass>();
}

[Serializable] 
    // Make sure everthing inside class is serializable
    // References probably won't work.
public class MyClass
{
    public Vector3 pos;
    public Color color;
    
}

[Serializable] 
    // Stuff in list in barrelType will maybe only care about base type (MyClass)
public class MyOtherClass : MyClass
{
    public Quaternion rot;

}
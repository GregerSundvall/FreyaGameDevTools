using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
public class ExplosiveBarrelManager : MonoBehaviour
{

    public static  List<ExplosiveBarrel> allTheBarrels = new List<ExplosiveBarrel>();

    #if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        foreach (var barrel in allTheBarrels)
        {
            Handles.DrawAAPolyLine(transform.position, barrel.transform.position);


        }
    }
    #endif
}

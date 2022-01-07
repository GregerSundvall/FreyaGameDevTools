using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
#if UNITY_EDITOR
using UnityEditor;
#endif
public class ExplosiveBarrelManager : MonoBehaviour
{

    public static  List<ExplosiveBarrel> allTheBarrels = new List<ExplosiveBarrel>();

    #if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Handles.zTest = CompareFunction.LessEqual;
        
        foreach (var barrel in allTheBarrels)
        {
            if (barrel.type == null)
            {
                continue;
            }
            Vector3 managerPos = transform.position;
            Vector3 barrelPos = barrel.transform.position;
            float halfHeight = (managerPos.y - barrelPos.y) * .5f;
            Vector3 offset = Vector3.up * halfHeight;

            Handles.DrawBezier(managerPos,
                barrelPos,
                managerPos - offset,
                barrelPos + offset,
                barrel.type.color,
                EditorGUIUtility.whiteTexture,
                1f);
            
            // Handles.DrawAAPolyLine(transform.position, barrel.transform.position);
        }
        Handles.color = Color.white;
    }
    #endif
}

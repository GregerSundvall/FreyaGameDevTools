using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
[CanEditMultipleObjects]
[CustomEditor(typeof(BarrelType))]
public class BarrelTypeEditor : Editor
{
    
    SerializedObject so;
    private SerializedProperty propRadius;
    private SerializedProperty propDamage;
    private SerializedProperty propColor;
    
    private void OnEnable()
    {
        so = serializedObject;
        propRadius = so.FindProperty("radius");
        propDamage = so.FindProperty("damage");
        propColor = so.FindProperty("color");
        
    }

    public override void OnInspectorGUI()
    {
        so.Update();
        EditorGUILayout.PropertyField(propRadius);
        EditorGUILayout.PropertyField(propDamage);
        EditorGUILayout.PropertyField(propColor);
        //so.ApplyModifiedProperties();
        if (so.ApplyModifiedProperties())
        {
            //if something changed
            ExplosiveBarrelManager.UpdateAllBarrelColors();
        }
        
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BarrelType))]
public class BarrelTypeEditor : Editor
{
    public enum Things
    {
        Bleep, Bloop, Blap
    }

    Things things;
    float someValue;
    Transform tf;
    
    
    public override void OnInspectorGUI()
    {

        using (new GUILayout.VerticalScope(EditorStyles.helpBox))
        {
            GUILayout.Label("Heading", EditorStyles.boldLabel);    
            using (new GUILayout.HorizontalScope())
            {
                GUILayout.Label("Things", GUILayout.Width(60));
                GUILayout.Label("Noot");
                if (GUILayout.Button("Do a thing"))
                {
                    Debug.Log("Did the thing");
                }
            }
            EditorGUILayout.ObjectField("Assign tranform here",null, typeof(Transform), true);
        }
        

        
        GUILayout.Label("Noot", EditorStyles.toolbar);
        GUILayout.Space(40);
        GUILayout.Label("Noot", GUI.skin.button); // LOOKS like a button...


        things = (Things)EditorGUILayout.EnumPopup(things);
        //someValue = GUILayout.HorizontalSlider(someValue, -1f, 1f);
        
        
        // Explicit positioning, using rect:
        // GUI              
        // EditorGUI
        
        // "Auto layout"
        //GUILayout         
        //EditorGUILayout
        
        // base.OnInspectorGUI();
    }
}

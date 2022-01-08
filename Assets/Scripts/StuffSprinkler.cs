using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

public class StuffSprinkler : EditorWindow
{
    [MenuItem("Tools/StuffSprinkler")]
    public static void OpenSprinkler() => GetWindow<StuffSprinkler>();

    public float radius = 2f;
    public int spawnCount = 9;

    private SerializedObject so;
    private SerializedProperty propRadius;
    private SerializedProperty propSpawnCount;

    private Vector2[] randPoints;
    
    
    private void OnEnable()
    {
        so = new SerializedObject(this);

        propRadius = so.FindProperty("radius");
        propSpawnCount = so.FindProperty("spawnCount");
        
        SceneView.duringSceneGui += DuringSceneGUI;
        
        GenerateRandomPoints();
    }    
    private void OnDisable()
    {
        SceneView.duringSceneGui -= DuringSceneGUI;
    }
    
    
    void GenerateRandomPoints()
    {
        randPoints = new Vector2[spawnCount];
        for (int i = 0; i < spawnCount; i++)
        {
            randPoints[i] = Random.insideUnitCircle;
        }
        
        
    }


    private void OnGUI()
    {
        so.Update();
        EditorGUILayout.PropertyField(propRadius);
        EditorGUILayout.PropertyField(propSpawnCount);
        if(so.ApplyModifiedProperties())
        {
            // In if with repaintall bc may be low fps otherwise
            SceneView.RepaintAll();
        }
    }


    void DrawSphere(Vector3 pos)
    {
        Handles.SphereHandleCap(-1, pos, quaternion.identity, 0.1f, EventType.Repaint);
    }

    void DuringSceneGUI(SceneView sceneView)
    {
        Transform camTf = sceneView.camera.transform;

        Ray ray = new Ray(camTf.position, camTf.forward);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            // Setting up tangent space
            Vector3 hitNormal = hit.normal;
            Vector3 hitTangent = Vector3.Cross(hitNormal, camTf.up).normalized;
            Vector3 hitBitangent = Vector3.Cross(hitNormal, hitTangent);

            foreach (var p in randPoints)
            {
                Vector3 worldPos = hit.point + (hitTangent * p.x + hitBitangent * p.y) * radius;
                
                DrawSphere(worldPos);
            }
            
            Handles.color = Color.red;
            Handles.DrawAAPolyLine(10, hit.point, hit.point + hitTangent);
            Handles.color = Color.green;
            Handles.DrawAAPolyLine(10, hit.point, hit.point + hitBitangent);
            Handles.color = Color.blue;
            Handles.DrawAAPolyLine(10, hit.point, hit.point + hitNormal);
            Handles.color = Color.white;
            
            Handles.DrawWireDisc(hit.point, hit.normal, radius);
        }
        
        
        
        
        
    }
}

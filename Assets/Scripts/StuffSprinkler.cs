using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using Random = UnityEngine.Random;

public class StuffSprinkler : EditorWindow
{
    [MenuItem("Tools/StuffSprinkler")]
    public static void OpenSprinkler() => GetWindow<StuffSprinkler>();
    
    float TAU = Mathf.PI * 2;

    public float radius = 2f;
    public int spawnCount = 9;
    public GameObject spawnPrefab;
    public Material previewMaterial;

    private SerializedObject so;
    private SerializedProperty propRadius;
    private SerializedProperty propSpawnCount;
    private SerializedProperty propSpawnPrefab;
    private SerializedProperty propPreviewMaterial;

    private Vector2[] randPoints;
    
    
    private void OnEnable()
    {
        so = new SerializedObject(this);

        propRadius = so.FindProperty("radius");
        propSpawnCount = so.FindProperty("spawnCount");
        propSpawnPrefab = so.FindProperty("spawnPrefab");
        propPreviewMaterial = so.FindProperty("previewMaterial");
        
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
        propRadius.floatValue = propRadius.floatValue.AtLeast(1);
        EditorGUILayout.PropertyField(propSpawnCount);
        propSpawnCount.intValue = propSpawnCount.intValue.AtLeast(1);
        EditorGUILayout.PropertyField(propSpawnPrefab);
        EditorGUILayout.PropertyField(propPreviewMaterial);
        

        if(so.ApplyModifiedProperties())
        {
            // In if with repaintall bc may be low fps otherwise
            GenerateRandomPoints();
            SceneView.RepaintAll();
        }

        // Deselect value box if you click somewhere else
        if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
        {
            GUI.FocusControl(null);
            Repaint(); // Needed in some cases
        }
    }


    void DrawSphere(Vector3 pos) {
        Handles.SphereHandleCap(-1, pos, quaternion.identity, 0.1f, EventType.Repaint);
    }
    
    void TrySpawnObjects(List<Pose> poses)
    {
        if (spawnPrefab == null)
        {
            return;
        }

        foreach (var pose in poses)
        {
            //spawn prefab
            GameObject spawnedThing = (GameObject)PrefabUtility.InstantiatePrefab(spawnPrefab);
            Undo.RegisterCreatedObjectUndo(spawnedThing, "Spawn objects");
            spawnedThing.transform.position = pose.position;
        }
        GenerateRandomPoints(); // Generate new preliminary points
    }

    void DuringSceneGUI(SceneView sceneView)
    {
        Handles.zTest = CompareFunction.LessEqual;
        
        Transform camTf = sceneView.camera.transform;

        // Ray from mouse pos
        if (Event.current.type == EventType.MouseMove) // Make sure it repaints on mouse move.
        {
            sceneView.Repaint();
        }

        bool holdingAlt = (Event.current.modifiers & EventModifiers.Alt) != 0;
        
        // change radius
        if (Event.current.type == EventType.ScrollWheel && holdingAlt == false)
        {
            float scrollDirection = Mathf.Sign(Event.current.delta.y);
            so.Update();
            propRadius.floatValue *= 1f + scrollDirection * 0.05f;
            so.ApplyModifiedProperties();
            Repaint(); // Update editor window
            Event.current.Use(); // Consume event, dont let it get picked up by anything else (like zoom)
        }

        Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);

        // Ray from camera
        //Ray ray = new Ray(camTf.position, camTf.forward);
        
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            // Setting up tangent space
            Vector3 hitNormal = hit.normal;
            Vector3 hitTangent = Vector3.Cross(hitNormal, camTf.up).normalized;
            Vector3 hitBitangent = Vector3.Cross(hitNormal, hitTangent);

            Ray GetTangentRay(Vector2 tangentSpacePos)
            {
                Vector3 rayOrigin = hit.point + (hitTangent * tangentSpacePos.x + hitBitangent * tangentSpacePos.y) * radius;
                rayOrigin += hitNormal * 2; // raycast offset (up)
                Vector3 rayDirection = -hitNormal;
                return new Ray(rayOrigin, rayDirection);
            }

            List<Pose> hitPoses = new List<Pose>();
            

            // Drawing points
            foreach (var p in randPoints)
            {
                // Create ray for this point
                Ray ptRay = GetTangentRay(p);
                // Raycast to find point on surface
                if (Physics.Raycast(ptRay, out RaycastHit ptHit))
                {
                    // Calculate rotation assign to pose together with position.
                    Quaternion randomRotation = quaternion.Euler(0, 0, Random.value * 360);
                    Quaternion rot = Quaternion.LookRotation(ptHit.normal) * (randomRotation * Quaternion.Euler(90f,0f,0f));
                    Pose pose = new Pose(ptHit.point, rot);
                    hitPoses.Add(pose);
                    
                    // Draw sphere and normal on surface
                    DrawSphere(ptHit.point);
                    Handles.DrawAAPolyLine(ptHit.point, ptHit.point + ptHit.normal);
                    
                    //Mesh
                    MeshFilter[] filters = spawnPrefab.GetComponentsInChildren<MeshFilter>();
                    foreach (var filter in filters)
                    {
                        Mesh mesh = filter.sharedMesh;
                        Material mat = filter.GetComponent<MeshRenderer>().sharedMaterial;
                        mat.SetPass(0);
                        Graphics.DrawMeshNow(mesh, pose.position, pose.rotation);
                    }
                    
                    // Mesh mesh = spawnPrefab.GetComponent<MeshFilter>().sharedMesh;
                    // Material mat = spawnPrefab.GetComponent<MeshRenderer>().sharedMaterial;
                    
                    
                    
                }
            }
            

            
            // spawn on Press
            if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Space)
            {
                Debug.Log(Event.current.type);
                Debug.Log("Spawn");
                TrySpawnObjects(hitPoses);
            }
            
            
            Handles.color = Color.red;
            Handles.DrawAAPolyLine(10, hit.point, hit.point + hitTangent);
            Handles.color = Color.green;
            Handles.DrawAAPolyLine(10, hit.point, hit.point + hitBitangent);
            Handles.color = Color.blue;
            Handles.DrawAAPolyLine(10, hit.point, hit.point + hitNormal);
            Handles.color = Color.white;
            Handles.DrawAAPolyLine(6, hit.point, hit.point + hitNormal);

            // Draw circle adapted to terrain
            const int circleDetail = 128;
            Vector3[] ringPoints = new Vector3[circleDetail];
            for (int i = 0; i < circleDetail; i++)
            {
                float t = i / (float) circleDetail - 1;
                float angRad = t * TAU;
                Vector2 dir = new Vector2(Mathf.Cos(angRad), Mathf.Sin(angRad));
                Ray r = GetTangentRay(dir);
                if (Physics.Raycast(r, out RaycastHit cHit))
                {
                    ringPoints[i] = cHit.point + cHit.normal * 0.02f;
                }
                else
                {
                    ringPoints[i] = r.origin;
                }
            }
            Handles.DrawAAPolyLine(ringPoints);
            
            // Handles.DrawWireDisc(hit.point, hit.normal, radius);
        }
        
        
        
        
    }
}

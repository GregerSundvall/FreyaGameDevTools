using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

public class SnapperTool : EditorWindow
{
    [MenuItem("Tools/Snapper Tool")]
    public static void OpenTheThing() => GetWindow<SnapperTool>("Snapper");

    float TAU = Mathf.PI * 2;

    
    public float gridSize = 1f;
    public GridType gridType = GridType.Cartesian;
    public int angularDivisions = 24;
    
    private SerializedObject so;
    private SerializedProperty propGridSize;
    private SerializedProperty propGridType;
    private SerializedProperty propAngularDivisions;
    
    public enum GridType
    {
        Cartesian,
        Polar
    }

    private void OnEnable()
    {
        so = new SerializedObject(this);
        propGridSize = so.FindProperty("gridSize");
        propGridType = so.FindProperty("gridType");
        propAngularDivisions = so.FindProperty("angularDivisions");

        gridSize = EditorPrefs.GetFloat("Snapper_tool_gridSize", 1f);
        gridType = (GridType)EditorPrefs.GetInt("Snapper_tool_gridType", 0);
        angularDivisions = EditorPrefs.GetInt("Snapper_tool_angularDivisions", 24);

        Selection.selectionChanged += Repaint;
        SceneView.duringSceneGui += DuringSceneGUI; // SnapperTool is now drawing a line in scene.
    }
    private void OnDisable()
    {
        Selection.selectionChanged -= Repaint;
        SceneView.duringSceneGui -= DuringSceneGUI;

        EditorPrefs.SetFloat("Snapper_tool_gridSize", gridSize);
        EditorPrefs.SetInt("Snapper_tool_gridType", (int)gridType);
        EditorPrefs.SetInt("Snapper_tool_angularDivisions", angularDivisions);
    }

    // duringSceneGUI is the editors update which runs at like 100 fps
    void DuringSceneGUI(SceneView sceneView)
    {
        if (Event.current.type == EventType.Repaint) // Prevents running constantly.
        {
            Handles.zTest = CompareFunction.LessEqual;
            const float gridDrawExtent = 16;

            if (gridType == GridType.Cartesian)
            {
                DrawGridCartesian(gridDrawExtent);
            }
            else
            {
                DrawGridPolar(gridDrawExtent);
            }
        }
    }
    
    void DrawGridPolar(float gridDrawExtent)
    {
        int ringCount = Mathf.RoundToInt((gridDrawExtent / gridSize));
        float radiusOuter = (ringCount - 1) * gridSize;

        for (int i = 1; i < ringCount; i++)
        {
            Handles.DrawWireDisc(Vector3.zero, Vector3.up, gridSize * i);
        }

        for (int i = 0; i < angularDivisions; i++)
        {
            float t = i / (float)angularDivisions;
            float angRad = t * TAU;
            float x = Mathf.Cos(angRad);
            float y = Mathf.Sin(angRad);
            Vector3 dir = new Vector3(x, 0, y);
            
            Handles.DrawAAPolyLine(Vector3.zero, dir * radiusOuter);
        }
    }
    
    void DrawGridCartesian(float gridDrawExtent)
    {
        int lineCount = Mathf.RoundToInt((gridDrawExtent * 2) / gridSize);
        if (lineCount % 2 == 0)
        {
            lineCount++;
        }
        int halfLineCount = lineCount / 2;

        for (int i = 0; i < lineCount; i++)
        {
            int intOffset = i - halfLineCount;
            float xCoord = intOffset * gridSize;
            float zCoord0 = halfLineCount * gridSize;
            float zCoord1 = -halfLineCount * gridSize;
            
            Vector3 p0 = new Vector3(xCoord, 0f, zCoord0);
            Vector3 p1 = new Vector3(xCoord, 0f, zCoord1);
            Handles.DrawAAPolyLine(p0, p1);
            
            p0 = new Vector3(zCoord0, 0f, xCoord);
            p1 = new Vector3(zCoord1, 0f, xCoord);
            Handles.DrawAAPolyLine(p0, p1);
        }
    }

    void OnGUI()
    {
        so.Update();
        EditorGUILayout.PropertyField(propGridType);
        EditorGUILayout.PropertyField(propGridSize);
        if (gridType == GridType.Polar)
        {
            EditorGUILayout.PropertyField(propAngularDivisions);
            // if (propAngularDivisions.intValue < 4)
            // {
            //     propAngularDivisions.intValue = 4;
            // }
            propAngularDivisions.intValue = Mathf.Max(4, propAngularDivisions.intValue);
        }
        so.ApplyModifiedProperties();
        
        using (new EditorGUI.DisabledScope(Selection.gameObjects.Length == 0))
        {
            if (GUILayout.Button("Snap Selection"))
            {
                SnapSelection();
            }
        }
    }

    void SnapSelection()
    {
        foreach (var go in Selection.gameObjects)
        {
            // Save what you are changing into an undo string!
            Undo.RecordObject(go.transform, "snap objects");
            go.transform.position = GetSnappedPosition(go.transform.position);

        }
    }
    
    Vector3 GetSnappedPosition( Vector3 posOriginal)
    {
        if (gridType == GridType.Cartesian)
        {
            return posOriginal.Round(gridSize);
        }

        if (gridType == GridType.Polar)
        {
            Vector2 vec = new Vector2(posOriginal.x, posOriginal.z);
            
            float dist = vec.magnitude;
            float distSnapped = dist.Round(gridSize);

            float angRad = Mathf.Atan2(vec.y, vec.x);
            float angTurns = angRad / TAU;
            float angTurnsSnapped = angTurns.Round(1f / angularDivisions);
            // Same same: float angSnapped = Mathf.Round(angTurns * angularDivisions) / angularDivisions;
            float angRadSnapped = angTurnsSnapped * TAU;

            Vector2 dirSnapped = new Vector2(Mathf.Cos(angRadSnapped), Mathf.Sin(angRadSnapped));
            Vector2 snappedVec = dirSnapped * distSnapped;

            return new Vector3(snappedVec.x, posOriginal.y, snappedVec.y);
        }
        return default;
    }
}

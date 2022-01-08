using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

public class SnapperTool : EditorWindow
{

    public float gridSize = 1f;
    private SerializedObject so;
    private SerializedProperty propGridSize;

    [MenuItem("Tools/Snapper Tool")]
    public static void OpenTheThing() => GetWindow<SnapperTool>("Snapper");


    private void OnEnable()
    {
        so = new SerializedObject(this);
        propGridSize = so.FindProperty("gridSize");
        
        Selection.selectionChanged += Repaint;
        SceneView.duringSceneGui += DuringSceneGUI; // SnapperTool is now drawing a line in scene.
    }
    private void OnDisable()
    {
        Selection.selectionChanged -= Repaint;
        SceneView.duringSceneGui -= DuringSceneGUI;

    }

    // duringSceneGUI is the editors update which runs at like 100 fps
    void DuringSceneGUI(SceneView sceneView)
    {
        if (Event.current.type == EventType.Repaint) // Prevents running constantly.
        {
            Handles.zTest = CompareFunction.LessEqual;
        
            const float gridDrawExtent = 16;
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

    }

    void OnGUI()
    {
        so.Update();
        EditorGUILayout.PropertyField(propGridSize);
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
            go.transform.position = go.transform.position.Round(gridSize);

        }
    }
}

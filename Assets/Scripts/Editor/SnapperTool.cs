using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class SnapperTool : EditorWindow
{

    [MenuItem("Tools/Snapper Tool")]
    public static void OpenTheThing()
    {
        GetWindow<SnapperTool>("Snapper");
    }


    private void OnEnable()
    {
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
        Handles.DrawLine(Vector3.zero, Vector3.up);
    }

    void OnGUI()
    {
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
            go.transform.position = go.transform.position.Round();

        }
    }
}

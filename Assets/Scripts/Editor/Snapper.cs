using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class Snapper
{
    private const string UNDO_STR_SNAP = "Snap Objects";
        
    
    [MenuItem("Edit/Snap Selected Object %&#P")]
    public static void SnapTheThings()
    {
        foreach (var go in Selection.gameObjects)
        {
            // Save what you are changing into an undo string!
            Undo.RecordObject(go.transform, UNDO_STR_SNAP);
            go.transform.position = go.transform.position.Round();
        }
    }
    // Optional validation function. Make sure string is the same.
    [MenuItem("Edit/Snap Selected Object %&#P", isValidateFunction:true)]
    public static bool SnapTheThingsValidate()
    {
        return Selection.gameObjects.Length > 0;
    }
    
    

}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[ExecuteAlways]
public class ExplosiveBarrel : MonoBehaviour
{
    public BarrelType type;

    private MaterialPropertyBlock mpb;

    static readonly int shPropColor = Shader.PropertyToID("_Color");
    
    public MaterialPropertyBlock Mpb
    {
        get
        {
            if (mpb == null)
                mpb = new MaterialPropertyBlock();
            return mpb;
        }
    }

    public void TryApplyColor()
    {
        if (type == null)
        {
            return;
        }
        MeshRenderer rnd = GetComponent<MeshRenderer>();
        Mpb.SetColor(shPropColor, type.color);
        rnd.SetPropertyBlock(Mpb);
    }

    private void Awake()
    {
        // For Runtime, run ApplyColor here.
    }

    private void OnValidate()
    {
        TryApplyColor();
    }

    private void OnEnable()
    {
        TryApplyColor(); // Might not be needed bc it's run in OnValidate.
        ExplosiveBarrelManager.allTheBarrels.Add(this);
    }

    private void OnDisable() => ExplosiveBarrelManager.allTheBarrels.Remove(this);

    private void OnDrawGizmos()
    {
        if (type == null)
        {
            return;
        }
        // Gizmos.DrawWireSphere(transform.position, radius);

        Handles.color = type.color;
        Handles.DrawWireDisc(transform.position, Vector3.up, type.radius);
        Handles.color = Color.white; // Reset color to not apply it to every handle.
    }
}

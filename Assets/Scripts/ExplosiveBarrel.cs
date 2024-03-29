using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class ExplosiveBarrel : MonoBehaviour
{
    [Range(1f, 8f)]
    public float radius = 1;
    
    public float damage = 10;
    public Color color = Color.red;

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

    void ApplyColor()
    {
        MeshRenderer rnd = GetComponent<MeshRenderer>();
        Mpb.SetColor(shPropColor, color);
        rnd.SetPropertyBlock(Mpb);
    }

    private void OnValidate()
    {
        ApplyColor();
    }

    private void OnEnable() => ExplosiveBarrelManager.allTheBarrels.Add(this);

    private void OnDisable() => ExplosiveBarrelManager.allTheBarrels.Remove(this);

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}

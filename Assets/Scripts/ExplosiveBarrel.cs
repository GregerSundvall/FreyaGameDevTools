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

    private void OnEnable() => ExplosiveBarrelManager.allTheBarrels.Add(this);

    private void OnDisable() => ExplosiveBarrelManager.allTheBarrels.Remove(this);

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}

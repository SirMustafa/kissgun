using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceZone : MonoBehaviour
{
    public enum UpdateMode
    {
        Manuel,
        FixedUpdate
    }
    
    [SerializeField] private UpdateMode updateMode = UpdateMode.FixedUpdate;
    [SerializeField] private LayerMask targetLayers;
    [SerializeField] private Vector3 center = Vector3.zero;
    [SerializeField] private Vector3 size = Vector3.one;
    [SerializeField] private Vector3 direction = Vector3.up;
    [SerializeField] private float force = 10f;
    [SerializeField] private bool drawGizmos = true;
    [SerializeField] private Color boundsColor = Color.green;
    [SerializeField] private Color directionColor = Color.blue;
    
    
    public void ApplyForce()
    {
        Collider[] colliders = Physics.OverlapBox(center + transform.position, size * 0.5f, transform.rotation, targetLayers);
        foreach (Collider coll in colliders)
        {
            Rigidbody rb = coll.attachedRigidbody;
            if (rb)
                rb.AddForceAtPosition(force * (transform.rotation * direction), transform.position + center);
        }
    }

    private void FixedUpdate()
    {
        if (updateMode == UpdateMode.FixedUpdate)
            ApplyForce();
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.matrix = transform.localToWorldMatrix;
        
        Gizmos.color = boundsColor;
        Gizmos.DrawWireCube(center, size);
        Gizmos.color = new Color(boundsColor.r, boundsColor.g, boundsColor.b, 0.25f);
        Gizmos.DrawCube(center, size);
        Gizmos.color = directionColor;
        DrawArrow.ForGizmo(center, direction * 1.25f, directionColor, 0.25f, 20f);
    }
#endif
}

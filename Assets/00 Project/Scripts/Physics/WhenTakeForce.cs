using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.Events;

public class WhenTakeForce : MonoBehaviour
{
    private Rigidbody rb;

    [SerializeField] private float targetForce;
    [SerializeField] private UnityEvent OnTakeTargetForce;
    
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.OnCollisionEnterAsObservable()
            .Where(collision => OnTakeTargetForce.GetPersistentEventCount() > 0 && collision.impulse.magnitude / Time.fixedDeltaTime >= targetForce)
            .Subscribe(collision =>
            {
                OnTakeTargetForce.Invoke();
            });
    }
}

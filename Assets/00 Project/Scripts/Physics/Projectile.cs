using System;
using MadLab.Utilities;
using Sirenix.OdinInspector;
using UniRx;
using UniRx.Triggers;
using UnityAtoms.BaseAtoms;
using UnityEngine;

public class Projectile : MonoBehaviour {
    [SerializeField] private Rigidbody rb;
    [SerializeField] private string hitImpactFx;
    [SerializeField] private string hitBodyImpactFx;
    [SerializeField] private FloatVariable speed;
    [SerializeField] private float deadTime;
    [SerializeField] private LayerMask targetLayers;
    [SerializeField] private bool useTrail = false;
    [SerializeField, ShowIf("$useTrail")] private TrailRenderer trail;

    private IDisposable collisionReactive;
    private IDisposable deadTimerReactive;
    
    private void OnEnable() {
        rb.velocity = transform.forward * speed.Value;

        collisionReactive = rb.OnTriggerEnterAsObservable()
            .Where(other => Utility.Contains(targetLayers, other.gameObject.layer))
            .Subscribe(other =>
            {
                bool isBody = other.CompareTag("Body");
                if (isBody)
                {
                    other.GetComponentInParent<NPCCharacter>()?.TakeLoveDamage(1);
                }
                
                SpawnImpactFx(isBody);
            });

        deadTimerReactive = Observable.Interval(TimeSpan.FromSeconds(deadTime))
            .Subscribe(_ =>
            {
                SpawnImpactFx(false);
            });
    }

    private void OnDisable()
    {
        rb.velocity = Vector3.zero;
        if (useTrail)
            trail.Clear();

        transform.position = Vector3.zero;
        collisionReactive.Dispose();
        deadTimerReactive.Dispose();
    }

    public void SpawnImpactFx(bool hitBody)
    {
        if ((hitBody ? hitBodyImpactFx : hitImpactFx) == "")
            return;
        
        ObjectPooler.Instance.SpawnFromPool(hitBody ? hitBodyImpactFx : hitImpactFx, transform.position, Quaternion.identity);
        gameObject.SetActive(false);
    }
}
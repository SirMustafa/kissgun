using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Explosion : MonoBehaviour
{
    [SerializeField, Range(0.5f, 5f)] private float radius = 1f;
    [SerializeField] private float explosionForce = 10f;
    [SerializeField] private float shakeMagnitude = 1f;

    [SerializeField, Range(0.25f, 1f)] private float damageRange = 0.5f;
    [SerializeField] private UnityEvent OnExplode;

    [SerializeField] private bool haveDamage = true;
    [SerializeField] private LayerMask forceLayers;
    [SerializeField] private LayerMask damageLayers;


    void OnEnable()
    {
        if (GameManager.GameStatus == GameStatus.WaitForStart)
            return;
        
        EZCameraShake.CameraShaker.Instance.ShakeOnce(shakeMagnitude, 4f, 0.1f, 1f);

        Collider[] objects = Physics.OverlapSphere(transform.position, radius * damageRange, damageLayers);

        if (haveDamage)
        {
            Health hp = null;
            foreach (Collider h in objects)
            {
                if (h.GetComponentInParent<Health>() != hp)
                {
                    hp = h.GetComponentInParent<Health>();

                    if (hp != null)
                    {
                        hp.TakeDamage(100);
                    }
                }
            }
        }

        if (OnExplode.GetPersistentEventCount() > 0)
            OnExplode.Invoke();
        
        objects = Physics.OverlapSphere(transform.position, radius, forceLayers);

        foreach (Collider h in objects)
        {
            Rigidbody r = h.GetComponent<Rigidbody>();
            if (r != null)
            {
                r.AddExplosionForce(explosionForce, transform.position, radius);
                if (!r.useGravity)
                    r.useGravity = true;
            }
        }
    }

#if UNITY_EDITOR


    void OnDrawGizmosSelected()
    {
        if (haveDamage)
        {
            Gizmos.color = new Color(1, 0, 0, 0.4f);
            Gizmos.DrawSphere(transform.position, radius * damageRange);
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, radius * damageRange);  
        }
        Gizmos.color = new Color(0, 1, 0, 0.2f);
        Gizmos.DrawSphere(transform.position, radius);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, radius);       
    }

#endif
}

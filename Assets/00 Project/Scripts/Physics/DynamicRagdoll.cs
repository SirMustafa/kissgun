using UnityEngine;
using System.Collections;
using MadLab.Utilities;
using Sirenix.OdinInspector;
using Sirenix.Utilities;

public class DynamicRagdoll : MonoBehaviour
{
    public enum RagdollMode
    {
        FromSelf,
        SeperateBody
    }

    [SerializeField] private RagdollMode mode = RagdollMode.FromSelf;

    [SerializeField] private Transform rig = null;
    [SerializeField, ShowIf("$mode", RagdollMode.SeperateBody)] private Transform targetRig = null;
    [SerializeField, ShowIf("$mode", RagdollMode.FromSelf)] private Animator anim = null;
    [SerializeField] private float totalMass = 60f;
    [SerializeField] private Rigidbody[] _rigidbodies;
    [SerializeField] private Collider[] _colliders;
    [SerializeField] private Transform[] extraObjectsInRagdoll;

    public Rigidbody[] RagdollParts()
    {
        return _rigidbodies;
    }
    
    public float TotalMass
    {
        private set {}
        get
        {
            return totalMass;
        }
    }
    
    bool isKinematics => _rigidbodies.Length > 0 ? _rigidbodies[0].isKinematic : false;
    
    [Button("Find Rigidbodies")]
    private void FindBodies()
    {
        _rigidbodies = rig.GetComponentsInChildren<Rigidbody>();
        _colliders = rig.GetComponentsInChildren<Collider>();
        anim = GetComponent<Animator>();
        
        totalMass = 0f;
        _rigidbodies.ForEach(x => totalMass += x.mass);
    }

    public void SetupRagdoll(Vector3 velocity, Transform parent = null)
    {
        transform.parent = parent;
        switch (mode)
        {
            case RagdollMode.FromSelf:
                SetKinematicsOfRigidbodies(false);
                
                if (velocity.magnitude > 0)
                {
                    for (int i = 0; i < _rigidbodies.Length; i++)
                    {
                        _rigidbodies[i].velocity += velocity * (_rigidbodies[i].mass / totalMass);
                    }
                }
                
                if (extraObjectsInRagdoll.Length > 0)
                {
                    foreach (Transform extraObject in extraObjectsInRagdoll)
                    {
                        extraObject.parent = transform;
                        Rigidbody extraRb = extraObject.GetComponent<Rigidbody>();
                        if (extraRb && extraRb.isKinematic)
                        {
                            extraRb.isKinematic = false;
                            extraRb.useGravity = true;
                            extraRb.GetComponent<Collider>().isTrigger = false;
                        }
                    }
                }
                
                break;
            
            case RagdollMode.SeperateBody:
                StartCoroutine(SeperateBodyOperations(velocity));
                break;
        }
    }

    public void SetVelocity(Vector3 velocity)
    {
        if (velocity.magnitude > 0)
        {
            for (int i = 0; i < _rigidbodies.Length; i++)
            {
                _rigidbodies[i].velocity += velocity * (_rigidbodies[i].mass / totalMass);
            }
        }
    }
    
    string buttonTitle => _rigidbodies.Length == 0 ? "Rigidbodies Not Founded" : isKinematics ? "Set UnKinematic" : "Set Kinematic";
    [Button("$buttonTitle")]
    private void SetKinematicsOfRigidbodies()
    {
        bool isKinematic = isKinematics;
        anim.enabled = !isKinematic;
        _rigidbodies.ForEach(x =>
        {
            x.isKinematic = !isKinematic;
            x.useGravity = isKinematic;
        });
        SetColliders(isKinematic);
    }

    public void SetColliders(bool active)
    {
        _colliders.ForEach(x =>
        {
            x.enabled = active;
        });
    }

    string setColliderButtonTitle => _colliders.Length == 0 ? "Colliders Not Founded" : _colliders[0].enabled ? "DeActive Colliders" : "SetActive Colliders";
    [Button("$setColliderButtonTitle")]
    private void SetCollider()
    {
        bool active = _colliders[0].enabled;
        _colliders.ForEach(x =>
        {
            x.enabled = !active;
        });
    }
    
    string setTriggerButtonTitle => _colliders.Length == 0 ? "Colliders Not Founded" : _colliders[0].isTrigger ? "UnTriggered Colliders" : "Triggered Colliders";
    [Button("$setTriggerButtonTitle")]
    private void SetTrigger()
    {
        bool active = _colliders[0].isTrigger;
        _colliders.ForEach(x =>
        {
            x.isTrigger = !active;
        });
    }
    
    private void SetKinematicsOfRigidbodies(bool isKinematic)
    {
        anim.enabled = isKinematic;
        _rigidbodies.ForEach(x =>
        {
            x.isKinematic = isKinematic;
            x.useGravity = !isKinematic;
        });
        _colliders.ForEach(x =>
        {
            x.enabled = !isKinematic;
            x.isTrigger = false;
        });
    }

    IEnumerator SeperateBodyOperations(Vector3 v)
    {
        Transform[] ragdollParts = rig.GetComponentsInChildren<Transform>();
        Transform[] targetBodyParts = targetRig.GetComponentsInChildren<Transform>();

        for (int i = 0; i < ragdollParts.Length; i++)
        {
            Transform targetBodyPart = Utility.GetParamWithNameFromArray(ragdollParts[i], targetBodyParts) as Transform;
            ragdollParts[i].position = targetBodyPart.position;
            ragdollParts[i].rotation = targetBodyPart.rotation;
        }

        rig.gameObject.SetActive(true);

        if (extraObjectsInRagdoll.Length > 0)
        {
            foreach (Transform extraObject in extraObjectsInRagdoll)
            {
                extraObject.parent = transform;
            }
        }


        if (v.magnitude > 0)
        {
            Rigidbody[] rigidbodies = rig.GetComponentsInChildren<Rigidbody>();

            for (int i = 0; i < rigidbodies.Length; i++)
            {
                rigidbodies[i].velocity += v * (rigidbodies[i].mass / totalMass);
            }
        }

        yield return null;
    }
}

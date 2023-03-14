using UnityEngine;

public class Follow : OptimizedUpdate
{
    [SerializeField] private Transform target;

    void Start()
    {
        transform.position = target.position;
        SetupOptimizedUpdate();
    }

    public override void UpdateMe(float deltaTime)
    {
        if (transform.position != target.position)
            transform.position = target.position;
    }
}
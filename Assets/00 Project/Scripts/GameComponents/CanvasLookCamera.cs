using UnityEngine;

public class CanvasLookCamera : OptimizedUpdate
{
    void Start()
    {
        SetupOptimizedUpdate();
    }

    public override void UpdateMe(float deltaTime)
    {
        transform.eulerAngles = Camera.main.transform.eulerAngles;
    }
}

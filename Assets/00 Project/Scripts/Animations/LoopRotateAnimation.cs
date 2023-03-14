using UnityEngine;

public class LoopRotateAnimation : OptimizedUpdate
{
    [SerializeField] private Vector3 rotation = Vector3.one;
    [SerializeField] private float speed = 1f;
    [SerializeField] private bool randomStart = true;
    private float startValue;
    private Vector3 startRot;

    public override void UpdateMe(float deltaTime)
    {
        transform.eulerAngles = startRot + rotation * Mathf.Sin(speed * (Time.time + startValue));
    }
}

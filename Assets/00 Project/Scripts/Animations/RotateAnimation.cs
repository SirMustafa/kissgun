using UnityEngine;

public class RotateAnimation : OptimizedUpdate
{
    [SerializeField] private bool sineRotate = false;
    [SerializeField] private bool rotateOnStart = false;
    [SerializeField] private Vector3 rotation = Vector3.zero;
    [SerializeField] private float rotateSpeed = 1f;


    [SerializeField] private bool randomStart = true;
    private float startValue;
    private Vector3 startRot;

    private void Start() 
    {
        if (rotateOnStart)
            Setup();
    }

    public void Setup()
    {
        SetupOptimizedUpdate();

        if (sineRotate)
            return;

        startRot = transform.eulerAngles;
        startValue = randomStart ? Random.Range(0, Mathf.PI) : 0;
    }

    public override void UpdateMe(float deltaTime)
    {
        if (sineRotate)
        {
            transform.eulerAngles = startRot + rotation * Mathf.Sin(rotateSpeed * (Time.time + startValue));
        }
        else
        {
            transform.Rotate(rotation * rotateSpeed * deltaTime);
        }
    }
}
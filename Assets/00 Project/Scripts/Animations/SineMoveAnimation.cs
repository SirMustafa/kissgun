using UnityEngine;

public class SineMoveAnimation : OptimizedUpdate
{
    [SerializeField] private Vector3 movePath;
    [SerializeField] private Transform body;
    [SerializeField] private bool moveOnStart = false;
    [SerializeField] private float speed;
    float animationTime = 0f;

    private void Start() 
    {
        if(moveOnStart)
        {
            Setup(animationTime);
        }
    }

    public void Setup(float animationTime)
    {
        this.animationTime = animationTime;
        SetupOptimizedUpdate();
    }

    Vector3 lastIncreaseValue = Vector3.zero;
    public override void UpdateMe(float deltaTime)
    {
        Vector3 increaseValue = movePath * Mathf.Sin((Time.time + animationTime) * speed);
        body.localPosition += increaseValue - lastIncreaseValue;
        lastIncreaseValue = increaseValue;
    }

#if UNITY_EDITOR
    public float moveSpeed()
    {
        return speed;
    }

    public float pathLenght()
    {
        return movePath.y;
    }
#endif
}

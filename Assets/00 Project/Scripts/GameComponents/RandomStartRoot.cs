using UnityEngine;

public class RandomStartRoot : MonoBehaviour
{
    float zAngle = 0f;
    void Start()
    {
        zAngle = Random.Range(0, 360f);
    }
}

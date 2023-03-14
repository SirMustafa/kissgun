using UnityEngine;
using System.Collections;

[RequireComponent(typeof (CanvasGroup))]
public class UIGhostAnimaton : OptimizedUpdate
{
    private CanvasGroup alphaChannel;
    [SerializeField, Range(0f, 1f)] private float minValue = 0.5f;
    [SerializeField] private float speed;

    private void Start() 
    {
        alphaChannel = GetComponent<CanvasGroup>();
        SetupOptimizedUpdate();
        //StartCoroutine(GhostAnim());
    }

    public override void UpdateMe(float deltaTime)
    {
        alphaChannel.alpha = Mathf.PingPong(Time.time * speed, 1 - minValue) + minValue;
    }

    IEnumerator GhostAnim()
    {
        float t = 0f;
        while (gameObject.activeInHierarchy)
        {
            while (alphaChannel.alpha != 1)
            {
                t += speed * Time.deltaTime; 
                t = Mathf.Clamp01(t);
                alphaChannel.alpha = Mathf.Lerp(minValue, 1, t);
                yield return null;
            }
            while (alphaChannel.alpha != minValue)
            {
                t -= speed * Time.deltaTime;
                t = Mathf.Clamp01(t);
                alphaChannel.alpha = Mathf.Lerp(minValue, 1, t);
                yield return null;
            }
            yield return null;
        }
    }
}

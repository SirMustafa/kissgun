using UnityEngine;

public class DeActivator : MonoBehaviour
{
    [SerializeField] private float time;

    public void OnEnable()
    {
        Invoke("DeActive", time);
    }
    
    private void DeActive()
    {
        gameObject.SetActive(false);
    }
}

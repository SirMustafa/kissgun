using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;

[HideMonoScript]
public class Health : MonoBehaviour {
    
    [TabGroup("Health")]
    [ProgressBar(0, 100, ColorMember = "GetHealthColor"), HideLabel, Space(5f)] public int health = 10;
    
    [TabGroup("Events")]
    [SerializeField, Space(5f)] private UnityEvent OnTakeDamage;
    [TabGroup("Events")]
    [SerializeField, Space(5f)] private UnityEvent OnDie;

    [HideInInspector] public bool isAlive = true;
    
    public void TakeDamage(int damage)
    {
        health = Mathf.Clamp(health - damage, 0, 100);

        if (OnTakeDamage.GetPersistentEventCount() > 0)
            OnTakeDamage.Invoke();

        if (health == 0)
        {
            isAlive = false;
            if (OnDie.GetPersistentEventCount() > 0)
                OnDie.Invoke();
        }
    }

    private Color GetHealthColor(int value) { return Color.Lerp(Color.red, Color.green, value / 100f); }
}
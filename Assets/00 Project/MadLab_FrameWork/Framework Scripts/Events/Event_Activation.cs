using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;

public class Event_Activation : MonoBehaviour
{
    [FoldoutGroup("On Enable"), HideLabel, SerializeField] private UnityEvent onEnable;
    [FoldoutGroup("On Disable"), HideLabel, SerializeField] private UnityEvent onDisable;

    private void OnEnable() {
        onEnable.Invoke();
    }

    private void OnDisable() {
        onDisable.Invoke();
    }
}

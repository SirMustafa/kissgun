using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine;

public class UIPointerHandler : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private UnityEvent PointerUpEvent;

    public void OnPointerClick(PointerEventData eventData)
    {
        PointerUpEvent.Invoke();
    }
}

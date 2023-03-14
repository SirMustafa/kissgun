using System;
using System.Net.NetworkInformation;
using DG.Tweening;
using UniRx;
using UnityAtoms.BaseAtoms;
using UnityEngine;

public class Crosshair : MonoBehaviour
{
    [SerializeField] private Vector2Event onPointerDrag;
    [SerializeField] private RectTransform crosshair;
    [SerializeField] private float sensitivity;
    [SerializeField] private Vector2Variable position;
    [SerializeField] private Vector2 border;

    private Vector2 screenSizeFixed;
    private Vector2 canvasHalf;
    private Vector2 crossPosition;
    private IDisposable updater;
    
    void Start()
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
        screenSizeFixed.x = Screen.width / rectTransform.rect.width;
        screenSizeFixed.y = Screen.height / rectTransform.rect.height;
        canvasHalf = rectTransform.rect.size * 0.5f;
        position.Value = (crosshair.anchoredPosition + canvasHalf) * screenSizeFixed;
        
        Setup();
    }

    #region Input Events

    private void OnPointerDrag(Vector2 inputData)
    {
        crossPosition += inputData * sensitivity;
        float x = canvasHalf.x - border.x;
        float y = canvasHalf.y - border.y;
        crossPosition = new Vector2(Mathf.Clamp(crossPosition.x, -x, x),
            Mathf.Clamp(crossPosition.y, -y, y));
    }

    #endregion

    public void ResetToCenter()
    {
        updater?.Dispose();
        onPointerDrag.Unregister(OnPointerDrag);
        crosshair.DOLocalMove(Vector3.zero, 0.5f)
            .OnComplete(() =>
            {
                crossPosition = crosshair.anchoredPosition;
                position.Value = (crosshair.anchoredPosition + canvasHalf) * screenSizeFixed;
            });
    }

    public void Setup()
    {
        onPointerDrag.Register(OnPointerDrag);
        updater = Observable.EveryUpdate()
            .TakeUntilDisable(this)
            .Where(_ => crosshair.anchoredPosition != crossPosition)
            .Subscribe(_ =>
            {
                crosshair.anchoredPosition = Vector2.Lerp(crosshair.anchoredPosition, crossPosition, 10 * Time.deltaTime);
                position.Value = (crosshair.anchoredPosition + canvasHalf) * screenSizeFixed;
            });
    }
}

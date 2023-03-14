using UnityEngine.EventSystems;
using UnityEngine;
using UnityAtoms.BaseAtoms;
using System.Collections;
using MadLab.Utilities;
using DG.Tweening;

public class Joystick : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
{
    #region  Singleton
    public static Joystick instance = null;
    private void Awake() {
        instance = this;
    }
    #endregion

    [Range(0f, 200f)]
    [SerializeField] private float handlerRange = 75f;

    [SerializeField] private CanvasGroup background;
    [SerializeField] private Transform handler;
    [SerializeField] private float showSpeed = 2f;

    private bool isStarted = false;
    private bool animated;
    private bool showGraphics;
    private bool moveable;
    private Ease sizeAnimationEase;

    [SerializeField] private Vector2Variable inputData;
    private Vector2 joyCenter = Vector2.zero;
    private Vector2 handlerPoint = Vector2.zero;

    public bool joyInput { private set ; get ; } = false;
    Coroutine currentCoroutine = null;

    GameStartType startType;

    public void Setup(bool animated, bool showGraphics, bool moveable, Ease sizeAnimationEase, GameStartType startType) {
        this.animated = animated;
        this.showGraphics = showGraphics;
        this.moveable = moveable;
        this.sizeAnimationEase = sizeAnimationEase;
        this.startType = startType;

        if (!this.showGraphics)
            background.alpha = 0f;
    }

    Vector2 startPoint = Vector2.zero;
    public void OnPointerDown(PointerEventData eventData)
    {
        if (!isStarted && startType == GameStartType.Tap)
        {
            isStarted = true;
            GameManager.instance.StartGame();
        }

        startPoint = eventData.position;

        joyCenter = eventData.position;

        background.transform.position = joyCenter;
        handler.position = joyCenter;        

        SetJoystickVisibility(true);
    
        joyInput = true;
    }

    public void OnDrag(PointerEventData eventData)
    {
        handlerPoint = eventData.position;
        Vector2 direction = handlerPoint - joyCenter;

        Vector2 directionClamped = Vector2.ClampMagnitude(direction, handlerRange);

        float dis = Vector2.Distance(eventData.position, startPoint);
        if (!isStarted && startType == GameStartType.WhenSwipe && dis != 0)
        {
            isStarted = true;
            GameManager.instance.StartGame();
        }

        if (direction.magnitude > handlerRange) {
            joyCenter = eventData.position - directionClamped;
            background.transform.position = joyCenter;
        }
        
        handler.localPosition = directionClamped;
        
        inputData.Value = directionClamped / handlerRange;

    }

    public void OnPointerUp(PointerEventData eventData)
    {
        CloseControl();
    }

    public void ControlActiveSelf(bool activeSelf)
    {
        gameObject.SetActive(activeSelf);
        if (!activeSelf)
            CloseControl();
    }

    private void CloseControl() 
    {
        joyInput = false;
        
        SetJoystickVisibility(false);

        inputData.Value = Vector2.zero;
    }

    Sequence fadeSeq;
    Sequence sizeSeq;
    public void SetJoystickVisibility(bool isVisible){
        if (animated && showGraphics)
        {
            if (fadeSeq != null && fadeSeq.IsActive())
                fadeSeq.Kill();

            fadeSeq = DOTween.Sequence();
            fadeSeq.Append(
                background.DOFade(isVisible ? 1f : 0f, 0.5f)
            );

            if (sizeSeq != null && sizeSeq.IsActive())
                sizeSeq.Kill();
                
            sizeSeq = DOTween.Sequence();
            sizeSeq.Append(
                background.transform.DOScale(isVisible ? Vector3.one : Vector3.zero, 0.5f)
                    .SetEase(isVisible ? sizeAnimationEase : Ease.InSine)
            );
        }
        else if (showGraphics)
        {
            background.gameObject.SetActive(isVisible);
        }
    }
}

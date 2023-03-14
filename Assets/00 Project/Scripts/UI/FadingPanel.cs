using UnityEngine;
using DG.Tweening;

public class FadingPanel : MonoBehaviour
{
    #region Singleton
    public static FadingPanel instance = null;
    private void Awake() {
        instance = this;
    }
    #endregion

    CanvasGroup group;

    private void Start() {
        group = GetComponent<CanvasGroup>();
    }

    Sequence fadeSeq;
    public void Fade(float alpha, float duration, bool blocksRaycasts = false, bool interactable = false, Ease ease = Ease.Linear){
        if (fadeSeq != null && fadeSeq.IsActive())
            fadeSeq.Kill();
        
        fadeSeq = DOTween.Sequence();
        fadeSeq.Append(
            group.DOFade(alpha, duration)
                .SetEase(ease)
        );

        group.interactable = interactable;
        group.blocksRaycasts = blocksRaycasts;
    }
}

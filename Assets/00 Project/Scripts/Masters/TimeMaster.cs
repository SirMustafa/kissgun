using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class TimeMaster
{
    public static float timeMustBe = 1f;

    public static void SlowTime()
    {
        SetTimeScale(0.5f);
    }

    public static void NormalTime()
    {
        SetTimeScale(1f);
    }

    public static void FastTime()
    {
        SetTimeScale(1.5f);
    }

    static Sequence timeSeq;
    public static void SetTimeScale(float timeScale = 1f, float duration = 1f, Ease ease = Ease.OutCirc)
    {
        timeMustBe = timeScale;

        if (timeSeq != null && timeSeq.IsActive())
            timeSeq.Kill();

        timeSeq = DOTween.Sequence();
        timeSeq.Append
        (
            DOTween.To(() => Time.timeScale, x => Time.timeScale = x, timeScale, duration)
                .SetEase(ease)
                .OnUpdate(() => { Time.fixedDeltaTime = Time.timeScale * 0.02f; }).SetUpdate(UpdateType.Normal, true)
        );
    }

    public static void SetTimeDirectly(float timeScale = 1f)
    {
        if (timeSeq != null && timeSeq.IsActive())
        {
            timeSeq.Kill();
        }

        Time.timeScale = timeScale;
        Time.fixedDeltaTime = timeScale * 0.02f;
    }
}
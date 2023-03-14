using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityAtoms.BaseAtoms;
using MadLab;
using Sirenix.OdinInspector;

[RequireComponent(typeof(CanvasGroup))]
public class LevelLoader : MonoBehaviour
{
    public static LevelLoader instance = null;

    Coroutine currentCoroutine = null;
    private CanvasGroup fadePanel = null;

    [SerializeField] private float fadeSpeed = 1f;
    [SerializeField] private bool FadeWhenStart = true;
    [SerializeField] private bool loadLevelWhenStart = true;
    [SerializeField, ShowIf("$loadLevelWhenStart")] private bool loadSavedLevel = false;
    private bool showTargetLevel => loadLevelWhenStart && !loadSavedLevel;
    [SerializeField, ShowIf("$showTargetLevel")] private int targetLevel = 1;
    [SerializeField] private BoolEvent isLoadedEvent;

    private void Awake()
    {
        instance = this;
        fadePanel = GetComponent<CanvasGroup>();
        if (FadeWhenStart)
            fadePanel.alpha = 1;
    }

    private void Start() {
        if (loadLevelWhenStart)
            ChangeLevel(loadSavedLevel ? DataBase.SavedCurrentLevel : targetLevel);
    }

    /// <summary>
    /// Sahneyi başlat
    /// </summary>
    public void StartCurrentLevel()
    {
        if (!FadeWhenStart)
            return;
            
        fadePanel.blocksRaycasts = false;

        if (currentCoroutine != null)
            StopCoroutine(currentCoroutine);

        currentCoroutine = StartCoroutine(FadeIn());
    }

    /// <summary>
    /// Sahneyi değiştir
    /// </summary>
    public void ChangeLevel(int index)
    {
        fadePanel.blocksRaycasts = true;

        if (currentCoroutine != null)
            StopCoroutine(currentCoroutine);
        
        currentCoroutine = StartCoroutine(FadeOut(index));
    }
    
    /// <summary>
    /// Sahneyi değiştir
    /// </summary>
    public void ChangeLevelDirectly(int index)
    {
        if (currentCoroutine != null)
            StopCoroutine(currentCoroutine);
        
        fadePanel.blocksRaycasts = true;
        fadePanel.alpha = 1f;
        DG.Tweening.DOTween.KillAll();
        SceneManager.LoadScene(index);
    }

    IEnumerator FadeIn() // Görüntü kayboluyor
    {
        bool fadeSound = AudioMaster.instance && GameSettingsData.gameSettings.fadeSoundsWhenLevelsChanging;
        float volume = 0f;

        while (fadePanel.alpha != 0)
        {
            fadePanel.alpha -= fadeSpeed * Time.unscaledDeltaTime;
            fadePanel.alpha = Mathf.Clamp01(fadePanel.alpha);

            if (volume != 1f)
            {
                volume += fadeSpeed * Time.unscaledDeltaTime;
                volume = Mathf.Clamp01(volume);

                if (fadeSound)
                    AudioMaster.instance.SetMasterVolume(volume);
            }

            yield return null;
        }
    }

    IEnumerator FadeOut(int index) // Görüntü açılıyor
    {
        bool fadeSound = AudioMaster.instance && GameSettingsData.gameSettings.fadeSoundsWhenLevelsChanging;
        float volume = 0f;
        if (AudioMaster.instance)
            AudioMaster.instance.GetMasterVolume();

        isLoadedEvent.Raise();
        while (fadePanel.alpha != 1)
        {
            fadePanel.alpha += fadeSpeed * Time.unscaledDeltaTime;
            fadePanel.alpha = Mathf.Clamp01(fadePanel.alpha);

            if (volume != 0f)
            {
                volume -= fadeSpeed * Time.unscaledDeltaTime;
                volume = Mathf.Clamp01(volume);

                if (fadeSound)
                    AudioMaster.instance.SetMasterVolume(volume);
            }

            yield return null;
        }
        DG.Tweening.DOTween.KillAll();
        yield return null;
        SceneManager.LoadScene(index);
    }
}

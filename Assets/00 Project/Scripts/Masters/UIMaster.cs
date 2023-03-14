using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using MadLab;
using MadLab.Utilities;
using Sirenix.OdinInspector;
using DG.Tweening;
using UnityAtoms.BaseAtoms;

[System.Serializable]
public class UIArts
{
    [FoldoutGroup("Sound")]
    public Sprite soundOn;
    [FoldoutGroup("Sound")]
    public Sprite soundOff;

    [FoldoutGroup("Vibration")]
    public Sprite vibrationOn;
    [FoldoutGroup("Vibration")]
    public Sprite vibrationOff;

    [FoldoutGroup("Shopping")]
    public Sprite lockedButton;
    [FoldoutGroup("Shopping")]
    public Sprite selectButton;
    [FoldoutGroup("Shopping")]
    public Sprite selectedButton;

    [FoldoutGroup("Chapter")]
    public Sprite chapter;
    [FoldoutGroup("Chapter")]
    public Sprite completedChapter;
    [FoldoutGroup("Chapter")]
    public Sprite currentChapter;
    [FoldoutGroup("Chapter")]
    public Sprite levelCircle;
    [FoldoutGroup("Chapter")]
    public Sprite completedLevelCircle;
}

public class UIMaster : OptimizedUpdate
{
    #region Singleton
    public static UIMaster instance = null;
    private void Awake()
    {
        instance = this; 
    }
    #endregion
  
    #region Arts
    [BoxGroup("Arts", order: 1), HideLabel]
    [SerializeField] private UIArts arts;
    #endregion

    #region Animations

    [TabGroup("Animations")]
    [SerializeField] private RectTransform container;
    [TabGroup("Animations")]
    [SerializeField] private AnimationCurve moveAnimation;
    [TabGroup("Animations")]
    [SerializeField] private AnimationCurve pauseAnimation;
    [TabGroup("Animations")]
    [SerializeField] private float containerSpeed;
    Vector3 desiredContainerPos;

    #endregion

    #region Panels
   
    [TabGroup("Panels")]
    [SerializeField] private CanvasGroup panel_Menu;
    [TabGroup("Panels")]
    [SerializeField] private CanvasGroup panel_InGame;
    [TabGroup("Panels")]
    [SerializeField] private CanvasGroup panel_alwaysDisplay;
    [TabGroup("Panels")]
    [SerializeField] private CanvasGroup tutorialPanel;
    [TabGroup("Panels")]
    [SerializeField] private Animator panel_Paused;
    [TabGroup("Panels")]
    [SerializeField] private GameObject victoryPanel;
    [TabGroup("Panels")]
    [SerializeField] private GameObject gameOverPanel;
    [TabGroup("Panels")]
    public Crosshair crosshair;

    #endregion

    #region Pickups

    [TabGroup("Panels")]
    [SerializeField] private Text coinsT;
    [TabGroup("Panels")]
    [SerializeField] private GameObject coinsPanel;
    [TabGroup("Panels")]
    [SerializeField] private Image coinIcon;
    [TabGroup("Panels")]
    [SerializeField] private Text timerT;
    [TabGroup("Panels")]
    [SerializeField] private GameObject timerPanel;
    [TabGroup("Panels")]
    [SerializeField] private Image timerIcon;

    #endregion

    #region Level Progress And Ranking
    
    bool chapterSystemOnInFramework => GameManager.isChapterMode;
    bool chapterSystem = false;
    int chapterCount = 0;
    [TabGroup("Level Progress")]
    [SerializeField] private bool rankSystem = false;
    [TabGroup("Level Progress"), ShowIf("$chapterSystemOnInFramework")]
    [SerializeField] private Image[] chapters = new Image[0];
    [TabGroup("Level Progress"), ShowIf("$chapterSystemOnInFramework")]
    [SerializeField] private Slider chapterProgressBar;
    [TabGroup("Level Progress"), HideIf("$chapterSystemOnInFramework")]
    [SerializeField] private Slider levelProgressBar;
    [TabGroup("Level Progress")]
    [SerializeField] private Image nextLevelCircle;
    [TabGroup("Level Progress")]
    [SerializeField] private Text Text_currentLevel;
    [TabGroup("Level Progress")]
    [SerializeField] private Text Text_nextLevel;
    [TabGroup("Level Progress"), ShowIf("$rankSystem")]
    [SerializeField] private Text Text_Rank;
    [TabGroup("Level Progress"), ShowIf("$rankSystem")]
    [SerializeField] private Text Text_FinishRank;
    
    #endregion

    #region Buttons

    [TabGroup("Buttons")]
    [SerializeField] private Image[] soundButtons;

    [TabGroup("Buttons")]
    [SerializeField] private Image[] vibrationButtons;

    #endregion

    #region FX
    
    [TabGroup("FX")]
    [SerializeField] private GameObject confetties;
    [TabGroup("FX")]
    [SerializeField] private AnimationCurve collectFXanim;
    [TabGroup("FX")]
    [SerializeField] private Image collectFX_Vignette = null;
    [TabGroup("FX")]
    [SerializeField, Range(0, 1f)] private float vignetteInstentity = 0.3f;
    private Color vignetteColor;
    
    #endregion

    public IntEvent showAds;

    public void Setup()
    {
        Text_currentLevel.text = DataBase.currentLevelNo.ToString();
        Text_nextLevel.text = (DataBase.currentLevelNo + 1).ToString();
        chapterSystem = chapterSystemOnInFramework;

        if (GameSettingsData.coinsInGame)
        {
            coinIcon.sprite = GameSettingsData.coinSprite;
            coinsPanel.SetActive(true);
        }
        else if(coinsPanel.activeSelf)
        {
            coinsPanel.SetActive(false);
        }

        if (GameSettingsData.gameSettings.TimerInGame())
        {
            timerIcon.sprite = GameSettingsData.gameSettings.TimerSprite();
            timerPanel.SetActive(true);
        }
        else if(timerPanel.activeSelf)
        {
            timerPanel.SetActive(false);
        }

        if (chapterSystem) {
            chapterCount = GameManager.instance.chapterCount;
            for (int i = 0; i < chapterCount; i++)
            {
                chapters[i].gameObject.SetActive(true);
            }
            chapters[0].rectTransform.parent.GetComponent<HorizontalGroup>().UpdateGroup();

            chapterProgressBar.maxValue = chapterCount + 1;
        }

        UpdateProgressBar(0);
        UpdateCoins();
        
        if (collectFX_Vignette)
            vignetteColor = collectFX_Vignette.color;
        
        UpdateSoundButtons();
        UpdateVibrationButtons();
        SetupOptimizedUpdate();
    }

    #region - UI events -

    public void UpdateProgressBar(float progress, int rank = 1)
    {
        if (chapterSystem){
            int currentChapter = Mathf.RoundToInt(progress);

            chapterProgressBar.value = currentChapter + 1f;
            
            if (currentChapter < chapters.Length)
            {
                chapters[currentChapter].sprite = arts.currentChapter;
                chapters[currentChapter].rectTransform.DOPunchScale(Vector3.one * 0.5f, 0.5f, 1);
            }
            
            if (currentChapter - 1 >= 0) {
                chapters[currentChapter - 1].sprite = arts.completedChapter;
                chapters[currentChapter - 1].rectTransform.DOPunchScale(Vector3.one * 0.5f, 0.5f, 1);
            }
            if (chapterProgressBar.value != chapterProgressBar.maxValue)
                chapters[currentChapter].sprite = arts.currentChapter;
        }
        else
        {
            levelProgressBar.value = progress;
        }

        if (Text_Rank)
            Text_Rank.text = "<size=60>"+rank+"</size>" + " <size=40>Th</size>";
    }   
    
    public void UpdateProgressBar(float progress, float progressFillDuration)
    {
        if (chapterSystem){
            int currentChapter = Mathf.RoundToInt(progress);
            DOTween.To(() => chapterProgressBar.value, x => chapterProgressBar.value = x, currentChapter + 1, progressFillDuration)
                .OnComplete(() =>
                {
                    if (currentChapter < chapters.Length)
                    {
                        chapters[currentChapter].sprite = arts.currentChapter;
                        chapters[currentChapter].rectTransform.DOPunchScale(Vector3.one * 0.5f, 0.5f, 1);
                    }
                });
            
            if (currentChapter - 1 >= 0) {
                chapters[currentChapter - 1].sprite = arts.completedChapter;
                chapters[currentChapter - 1].rectTransform.DOPunchScale(Vector3.one * 0.5f, 0.5f, 1);
            }
            if (chapterProgressBar.value != chapterProgressBar.maxValue)
                chapters[currentChapter].sprite = arts.currentChapter;
        }
    }

    public void SetProgressMaxValue(float maxValue)
    {
        levelProgressBar.maxValue = maxValue;
    }

    public void SetRank(int rank) {
        if (!rankSystem)
            return;

        if (Text_Rank)
            Text_Rank.text = "<size=60>" + rank + "</size>"
                        + "<size=40>" + (rank == 1 ? "st" : rank == 2 ? "nd" : rank == 3 ? "rd" : "th") + "</size>";

        if (Text_FinishRank)
            Text_FinishRank.text = "<size=150>" + rank + "</size>"
                                 + "<size=100>" + (rank == 1 ? "st" : rank == 2 ? "nd" : rank == 3 ? "rd" : "th") + "</size>";
    }

    public void UpdateCoins()
    {
        coinsT.text = DataBase.coins >= 10000 ? DataBase.coins.ToString("00,0k") :
                      DataBase.coins >= 1000 ? DataBase.coins.ToString("0,0k") :
                      DataBase.coins.ToString();
    }

    Sequence tutorialSeq;
    public void SetVisibleTutorialPanel(bool isVisible, float duration = 1f)
    {
        if (tutorialSeq != null && tutorialSeq.IsActive())
            tutorialSeq.Kill();

        tutorialSeq = DOTween.Sequence();
        tutorialSeq.Append(
            tutorialPanel.DOFade(isVisible ? 1f : 0f, duration)
            .SetUpdate(true)
        );
    }

    #endregion

    #region - Settings Panel -

    /// <summary>
    /// Change Sound Settings
    /// </summary>
    public void ChangeSoundActivation()
    {
        int index = DataBase.sound ? 0 : 1;
        PlayerPrefs.SetInt("Sound Settings", index);
        UpdateSoundButtons();
    }

    /// <summary>
    /// Change Vibration settings
    /// </summary>
    public void ChangeVibrationActivation()
    {
        int index = DataBase.vibration ? 0 : 1;
        PlayerPrefs.SetInt("Vibration Settings", index);
        UpdateVibrationButtons();
    }

    private void UpdateVibrationButtons()
    {
        if (vibrationButtons.Length == 0)
            return;
        int vibration = DataBase.vibration ? 1 : 0;

        for (int i = 0; i < vibrationButtons.Length; i++)
            vibrationButtons[i].sprite = vibration == 1 ? arts.vibrationOn : arts.vibrationOff;
    }

    private void UpdateSoundButtons()
    {
        int sfx = DataBase.sound ? 1 : 0;
        for (int i = 0; i < soundButtons.Length; i++)
            soundButtons[i].sprite = sfx == 1 ? arts.soundOn : arts.soundOff;
    }

    #endregion

    #region - Game Events -

    public void StartGame()
    {
        StartCoroutine(Utility.FadeOut(panel_InGame, 2f));
        panel_InGame.interactable = true;
        panel_InGame.blocksRaycasts = true;
        StartCoroutine(Utility.FadeIn(panel_Menu, 2f));
        panel_Menu.blocksRaycasts = false;

        PlayerPrefs.SetInt("PlayCount_Level" + DataBase.currentLevelNo, DataBase.currentLevelPlayCount + 1);
    }

    public void CollectFX()
    {
        if (collectFX != null)
            StopCoroutine(collectFX);
        
        collectFX = StartCoroutine(CollectEffect());
    }
    Coroutine collectFX = null;
    IEnumerator CollectEffect()
    {
        float t = 0f;
        while(t != collectFXanim.length)
        {
            t += 1 * Time.deltaTime;
            t = Mathf.Clamp(t, 0, collectFXanim.length);
            float evaluatedTime = collectFXanim.Evaluate(t);
            vignetteColor.a = Mathf.Lerp(0, vignetteInstentity, evaluatedTime);
            Camera.main.fieldOfView = Mathf.Lerp(60f, 61.5f, evaluatedTime);
            collectFX_Vignette.color = vignetteColor;
            yield return null;
        }
    }

    public void PauseGame()
    {
        panel_Paused.SetBool("Visibility", true);
        StartCoroutine(Utility.Tween((x) => { Time.timeScale = x; Time.fixedDeltaTime = x * 0.02f; },
                                            Time.timeScale, 0f, 1f, pauseAnimation, TimeScaleType.UnScaled));
    }

    public void ResumeGame()
    {
        panel_Paused.SetBool("Visibility", false);
        StartCoroutine(Utility.Tween((x) => { Time.timeScale = x; Time.fixedDeltaTime = x * 0.02f; },
                                            Time.timeScale, TimeMaster.timeMustBe, 1f, pauseAnimation, TimeScaleType.UnScaled));
    }

    public void Menu()
    {
        NavigateTo(0);
    }

    Vector3 CurrentContainerPos = Vector3.zero;
    private void NavigateTo(int index)
    {
        switch (index)
        {
            case 0:
                desiredContainerPos = Vector3.zero;
                break;

            case 1:
                desiredContainerPos = Vector3.left * 1080;
                break;
        }

        if (desiredContainerPos == CurrentContainerPos)
            return;

        CurrentContainerPos = desiredContainerPos;

        if (moveContainer != null)
            StopCoroutine(moveContainer);

        moveContainer = StartCoroutine(MoveContainer());
    }

    Coroutine moveContainer = null;
    IEnumerator MoveContainer()
    {
        float t = 0f;
        Vector3 startPos = container.anchoredPosition3D;
        while (t < moveAnimation.length)
        {
            t += containerSpeed * Time.deltaTime;
            t = Mathf.Clamp(t, 0, moveAnimation.length);

            container.anchoredPosition3D = Vector3.Lerp(startPos, desiredContainerPos, moveAnimation.Evaluate(t));
            yield return null;
        }
    }
    
    public void Victory()
    {
        nextLevelCircle.sprite = arts.completedLevelCircle;

        victoryPanel.SetActive(true);
        confetties.SetActive(true);
        FadingPanel.instance.Fade(0.75f, 0.5f, true, true);
        StartCoroutine(Utility.FadeIn(panel_alwaysDisplay, 2f));
        StartCoroutine(Utility.FadeIn(panel_InGame, 2f));

        PlayerPrefs.SetInt("Current Level", DataBase.nextLevelIndex);
        if (DataBase.currentLevelIndex == DataBase.totalLevels)
            PlayerPrefs.SetInt("Games Completed", DataBase.gameCompletedCount + 1);
    }

    public void GameOver()
    {
        gameOverPanel.SetActive(true);
        FadingPanel.instance.Fade(0.5f, 0.5f);
        StartCoroutine(Utility.FadeIn(panel_alwaysDisplay, 2f));
        StartCoroutine(Utility.FadeIn(panel_InGame, 2f));
    }

    #endregion

    #region - Scene Events -

    public void Restart()
    {
        LevelLoader.instance.ChangeLevel(DataBase.currentLevelIndex);
    }

    public void NextLevel()
    {
        LevelLoader.instance.ChangeLevel(DataBase.nextLevelIndex);
    }
    
    public void ShowAds()
    {
        showAds.Raise();
    }

    #endregion
}
using System.Collections.Generic;
using UnityEngine;
using UnityAtoms.BaseAtoms;
using Sirenix.OdinInspector;

#if UNITY_EDITOR
using UnityEditor;
using System.IO;
#endif

[System.Serializable]
public enum MobileControlType
{
    Drag,
    Swipe,
    Swerve,
    TapTiming,
    Joystick
}

[System.Serializable]
public enum GameStartType
{
    Tap,
    WhenSwipe
}

public enum VariableTypes
{
    Int,
    Float,
    Bool,
    vector2,
    vector3
}

[System.Serializable]
public class CustomVariable
{
    [FoldoutGroup("$name")] public string name = "new Custom Variable";
    [FoldoutGroup("$name")] public VariableTypes type;
    [FoldoutGroup("$name"), LabelText("value"), ShowIf("$type", VariableTypes.Float)] public float floatValue;
    [FoldoutGroup("$name"), LabelText("value"), ShowIf("$type", VariableTypes.Int)] public int intValue;
    [FoldoutGroup("$name"), LabelText("value"), ShowIf("$type", VariableTypes.Bool)] public bool boolValue;
    [FoldoutGroup("$name"), LabelText("value"), ShowIf("$type", VariableTypes.vector2)] public Vector2 vector2Value;
    [FoldoutGroup("$name"), LabelText("value"), ShowIf("$type", VariableTypes.vector3)] public Vector3 vector3Value;


    public void GetValue(out float value)
    {
        value = type == VariableTypes.Float ? floatValue : 0f;
    }

    public void GetValue(out int value)
    {
        value = type == VariableTypes.Int ? intValue : 0;
    }

    public void GetValue(out bool value)
    {
        value = type == VariableTypes.Bool ? boolValue : false;
    }

    public void GetValue(out Vector2 value)
    {
        value = type == VariableTypes.vector2 ? vector2Value : Vector2.zero;
    }

    public void GetValue(out Vector3 value)
    {
        value = type == VariableTypes.vector3 ? vector3Value : Vector3.zero;
    }
}

public class GameSettings : ScriptableObject {

    #region  Level Settings

    [TabGroup("Level Settings"), Title("Level Settings", "@totalLevels", TitleAlignments.Split), SerializeField] private int loopStartLevel;
    public int LoopStartLevel() => loopStartLevel;
    [TabGroup("Level Settings")]
    public bool chapterMode;
    [TabGroup("Level Settings")]
    public bool singleSceneMode;
    [TabGroup("Level Settings"), ShowIf("$singleSceneMode")]
    public bool setCurrentLevel;
    public bool showCurrentLevel => singleSceneMode && setCurrentLevel;
    [TabGroup("Level Settings"), ShowIf("$showCurrentLevel"), MinValue(1), MaxValue("$maxLevel"),]
    public int currentLevel;
    int maxLevel => MadLab.GameSettingsData.totalLevels;
    string totalLevels => "Total Levels : " + maxLevel;

    #endregion

    #region Playing Settings

    [TabGroup("Playing Settings")]
    public MobileControlType MobileInputType;
    [TabGroup("Playing Settings"), ShowIf("$MobileInputType", MobileControlType.Joystick)]
    public bool animatedJoystick;
    [TabGroup("Playing Settings"), ShowIf("$ShowSizeAnimationEaseParam")]
    public DG.Tweening.Ease joystickSizeAnimation_Ease;
    [TabGroup("Playing Settings"), ShowIf("$MobileInputType", MobileControlType.Joystick)]
    public bool showJoystickGraphics;
    [TabGroup("Playing Settings"), ShowIf("$MobileInputType", MobileControlType.Joystick)]
    public bool movableJoystick;
    [TabGroup("Playing Settings"), ShowIf("$MobileInputType", MobileControlType.Swipe)]
    public bool reversedInputData;
    [TabGroup("Playing Settings")]
    public GameStartType GameStarterType;
    [TabGroup("Playing Settings"), PropertyRange(0f, 1f)]
    public float InputResetDuration;
    [TabGroup("Playing Settings")]
    public bool fadeSoundsWhenLevelsChanging = false;

    #endregion

    #region EventSettings

    [TabGroup("Events")]
    public BoolEvent gameStartEvent;
    [TabGroup("Events")]
    public BoolEvent gameCompletedEvent;
    [TabGroup("Events")]
    public BoolEvent gameOverEvent;

    #endregion

    #region Other Settings

    [TabGroup("Other Settings"), LabelText("Is SDK Enabled"), OnValueChanged("UpdateSDK")]
    public bool sdkEnabled = false;

    #endregion

    private bool ShowSizeAnimationEaseParam(){
        return MobileInputType == MobileControlType.Joystick && animatedJoystick;
    }

    [ToggleGroup("Coins"), SerializeField] private bool Coins;
    public bool CoinsInGame() => Coins;

    [ToggleGroup("Coins"), SerializeField, PreviewField(70, ObjectFieldAlignment.Right)] private Sprite coinSprite;
    public Sprite CoinSprite() => coinSprite;

    [ToggleGroup("Timer"), SerializeField] private bool Timer;
    public bool TimerInGame() => Timer;

    [ToggleGroup("Timer"), SerializeField, PreviewField(70, ObjectFieldAlignment.Right)] private Sprite timerSprite;
    public Sprite TimerSprite() => timerSprite;

#region Audio

    [ToggleGroup("Audio"), SerializeField] private bool Audio;
    public bool AudioInGame() => Audio;
    [ToggleGroup("Audio"), SerializeField] private List<SoundFX> SoundFx;
    public SoundFX GetSound(string tag) { return SoundFx.Find((x) => x.tag == tag); }
    public List<SoundFX> GetAllSounds() { return SoundFx; }

#endregion

#region Music

    [ToggleGroup("Music"), SerializeField] private bool Music;

    public bool MusicInGame() => Music;

    [ToggleGroup("Music"), SerializeField] private List<SoundFX> MusicFx;
    public SoundFX GetMusic(string tag) { return MusicFx.Find((x) => x.tag == tag); }

    public SoundFX GetRandomMusic()
    {
        if (MusicFx != null && MusicFx.Count > 0)
            return MusicFx[Random.Range(0, MusicFx.Count)];
        
        return null;
    }
    public List<SoundFX> GetAllMusics() { return MusicFx; }

#endregion

#if UNITY_EDITOR

    private void UpdateSDK()
    {
        string elephant_SDKPath = "Assets/Elephant";

        List<string> defineSymbols = new List<string>();
        string[] defines = new string[0];
        PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.iOS, out defines);
        defineSymbols = new List<string>(defines);

        if (sdkEnabled)
        {
            defineSymbols.Add("SDK_ENABLED");
            PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.iOS, defineSymbols.ToArray());
        }
        else
        {
            defineSymbols.Remove("SDK_ENABLED");
            PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.iOS, defineSymbols.ToArray());
        }

        Debug.Log("SDKs " + (sdkEnabled ? "enabled" : "disabled") + " successfully!");
    }

    public void Init()
    {
        foreach (SoundFX sound in SoundFx)
        {
            sound.RandomizeColor();
        }
    }
#endif
}

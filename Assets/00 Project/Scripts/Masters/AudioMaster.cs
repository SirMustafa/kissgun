using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using Sirenix.OdinInspector;
using MadLab;
using DG.Tweening;

[System.Serializable]
public enum SoundType {
    Music,
    SFX
}

[System.Serializable]
public class SoundFX
{
    [FoldoutGroup("$tag")] public string tag;
    [FoldoutGroup("$tag")] public AudioClip clip;
    [FoldoutGroup("$tag"), ProgressBar(0f, 1f, ColorMember = "barColor"), SerializeField] public float volume = 1f;
    #if UNITY_EDITOR
    private Color barColor = new Color(Random.Range(0.25f, 1f), Random.Range(0.25f, 1f), Random.Range(0.25f, 1f));

    public void RandomizeColor()
    {
        barColor = new Color(Random.Range(0.25f, 1f), Random.Range(0.25f, 1f), Random.Range(0.25f, 1f));
    }
    #endif
}

public class AudioMaster : MonoBehaviour
{
    #region Singleton
    public static AudioMaster instance = null;
    private void Awake() {
        instance = this; 
    }
    #endregion

    [SerializeField] private AudioMixer master;
    [SerializeField] private AudioMixerGroup music;
    [SerializeField] private AudioMixerGroup sfx;

    Dictionary<string, AudioSource> sources = new Dictionary<string, AudioSource>();

    void Start()
    {
        if (!GameSettingsData.gameSettings.AudioInGame())
            Destroy(gameObject);

        GameObject sourcesObject = new GameObject("Audio Sources");
        sourcesObject.transform.SetParent(transform);

        List<SoundFX> soundFXes = new List<SoundFX>(GameSettingsData.gameSettings.GetAllSounds());
        for(int i = 0; i < soundFXes.Count; i++)
        {
            AudioSource source = sourcesObject.AddComponent<AudioSource>();
            source.playOnAwake = false;
            source.clip = soundFXes[i].clip;
            source.outputAudioMixerGroup = sfx;
            source.volume = soundFXes[i].volume;
            sources.Add(soundFXes[i].tag, source);
        }

        SoundFX musicData = GameSettingsData.gameSettings.GetRandomMusic();
        if (musicData != null)
        {
            AudioSource musicSource = sourcesObject.AddComponent<AudioSource>();
            musicSource.playOnAwake = false;
            musicSource.loop = true;
            musicSource.clip = musicData.clip;
            musicSource.outputAudioMixerGroup = music;
            musicSource.volume = musicData.volume;
            musicSource.Play();
        }
    }

    public void Play(string tag)
    {
        if (!sources.ContainsKey(tag))
            return;

        sources[tag].Play();
    }

    public float GetMasterVolume()
    {
        float vol = 0f;
        if (master != null)
            master.GetFloat("Master Volume", out vol);
        return AdvancedMath.Remap(-80f, 0f, 0, 1f, vol);
    }

    public void SetMasterVolume(float volume)
    {
        master.SetFloat("Master Volume", AdvancedMath.Remap(0f, 1f, -80f, 0f, volume));
    }

    public void SetMusicVolume(float volume)
    {
        master.SetFloat("Music Volume", AdvancedMath.Remap(0f, 1f, -80f, 0f, volume));
    }

    public void SetMusicLowpass(float lowpass)
    {
        if (musicSeq != null && musicSeq.IsActive())
            musicSeq.Kill();

        master.SetFloat("Music Lowpass", AdvancedMath.Remap(0f, 1f, 1000f, 22000f, lowpass));
    }

    Sequence musicSeq;
    public void SetMusicLowpass(float lowpass, float duration)
    {
        float musicVol = 0f;
        master.GetFloat("Music Lowpass", out musicVol);
        musicVol = AdvancedMath.Remap(1000f, 22000f, 0f, 1f, musicVol);

        if (musicSeq != null && musicSeq.IsActive())
            musicSeq.Kill();

        musicSeq = DOTween.Sequence();
        musicSeq.Append(
            DOTween.To(() => musicVol, x => musicVol = x, lowpass, duration)
                .OnUpdate(() => {
                    master.SetFloat("Music Lowpass", AdvancedMath.Remap(0f, 1f, 1000f, 22000f, lowpass));
                })
        );
    }
}

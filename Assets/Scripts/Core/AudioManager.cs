using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Audio;

public enum SfxId { Grapple }
public enum MusicId { Title, Gameplay }
public class AudioManager : MonoBehaviour
{
    [SerializeField] AudioSource musicPlayer;
    [SerializeField] AudioSource sfxPlayer;
    [SerializeField] AudioMixer mixer;
    [SerializeField] List<SfxData> sfxList;
    [SerializeField] List<MusicData> musicList;

    Dictionary<SfxId, SfxData> sfxLookup;
    Dictionary<MusicId, MusicData> musicLookup;

    public static AudioManager i { get; private set; }

    // Start is called before the first frame update
    void Awake()
    {
        i = this;
    }

    void Start() 
    {
        sfxLookup = sfxList.ToDictionary(x => x.id);
        musicLookup = musicList.ToDictionary(x => x.id);
    }

    public void PlayMusic(AudioClip clip, bool loop=true)
    {
        if (clip == null) return;
        musicPlayer.clip = clip;
        musicPlayer.loop = loop;
        musicPlayer.Play();
    }

    public void PlayMusic(MusicId audioId, bool loop=true)
    {
        if (!musicLookup.ContainsKey(audioId)) return;
        var audioData = musicLookup[audioId];
        PlayMusic(audioData.clip);
    }

    public void PlaySfx(AudioClip clip)
    {
        if (clip == null) return;
        sfxPlayer.PlayOneShot(clip);
    }

    public void PlaySfx(SfxId audioId)
    {
        if (!sfxLookup.ContainsKey(audioId)) return;

        var audioData = sfxLookup[audioId];
        PlaySfx(audioData.clip);
    }

    public void SetMusicVolume(float sliderValue)
    {
        mixer.SetFloat("MusicVolume", Mathf.Log10(sliderValue) * 20);
    }

    public void SetSfxVolume(float sliderValue)
    {
        mixer.SetFloat("SFXVolume", Mathf.Log10(sliderValue) * 20);
    }
}

[System.Serializable]
public class SfxData : AudioData
{
    public SfxId id;
}
[System.Serializable]
public class MusicData: AudioData
{
    public MusicId id;
}
[System.Serializable]
public class AudioData
{
    public AudioClip clip;
}
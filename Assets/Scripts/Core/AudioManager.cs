using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Audio;

public enum SfxId { Shoot, Grappling, Ungrapple, UISelect, UIConfirm, UIPause, UIUnpause, BossShoot, PlayerHurt, BossHurt, DogHurt, PlayerDeath, 
                    TreatShot, FishShot, BubbleShot, Null, Jump }
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

        sfxLookup = sfxList.ToDictionary(x => x.id);
        musicLookup = musicList.ToDictionary(x => x.id);
    }

    public void StopMusic()
    {
        musicPlayer.Stop();
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
    
    public void PlayMusic()
    {
        musicPlayer.Play();
    }

    public void PauseMusic()
    {
        musicPlayer.Pause();
    }

    public void PlaySfx(AudioClip clip, bool loop=false)
    {
        if (clip == null) return;
        if (!loop)
        {
            sfxPlayer.loop = false;
            sfxPlayer.PlayOneShot(clip);
        }
        else
        {
            sfxPlayer.loop = true;
            sfxPlayer.clip = clip;
            sfxPlayer.Play();
        }
    }
    public IEnumerator PlaySfxDelayed(AudioClip clip, float delay)
    {
        yield return new WaitForSeconds(delay);
        sfxPlayer.PlayOneShot(clip);
    }
    public void StopSfx()
    {
        sfxPlayer.Stop();
    }

    public void PlaySfx(SfxId audioId, bool loop=false)
    {
        if (!sfxLookup.ContainsKey(audioId)) return;

        var audioData = sfxLookup[audioId];
        PlaySfx(audioData.clip, loop);
    }

    public void SetMusicVolume(float sliderValue)
    {
        mixer.SetFloat("MusicVolume", Mathf.Log10(sliderValue) * 20);
        PlayerPrefs.SetFloat("MusicVolume", sliderValue);
    }

    public void SetSfxVolume(float sliderValue)
    {
        mixer.SetFloat("SFXVolume", Mathf.Log10(sliderValue) * 20);
        PlayerPrefs.SetFloat("SFXVolume", sliderValue);
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
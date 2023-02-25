using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Audio;
using UnityEngine.Pool;

public enum SfxId
{
    Shoot, Grappling, Ungrapple, UISelect, UIConfirm, UIPause, UIUnpause, BossShoot, PlayerHurt, BossHurt, DogHurt, PlayerDeath,
    TreatShot, FishShot, BubbleShot, Null, Jump
}
public enum MusicId { Title, Gameplay }
public class AudioManager : MonoBehaviour
{
    [SerializeField] AudioSource musicPlayer;
    [SerializeField] AudioSource sfxPlayerPrefab;
    [SerializeField] AudioMixer mixer;
    [SerializeField] List<SfxData> sfxList;
    [SerializeField] List<MusicData> musicList;

    Dictionary<SfxId, SfxData> sfxLookup;
    Dictionary<MusicId, MusicData> musicLookup;
    ObjectPool<AudioSource> sfxPlayerPool;
    private List<AudioSource> loopingSfx = new List<AudioSource>();

    public static AudioManager i { get; private set; }

    // Start is called before the first frame update
    void Awake()
    {
        i = this;

        sfxLookup = sfxList.ToDictionary(x => x.id);
        musicLookup = musicList.ToDictionary(x => x.id);

        sfxPlayerPool = new ObjectPool<AudioSource>(
            () => Instantiate(sfxPlayerPrefab, transform),
            audioSource => audioSource.enabled = true,
            audioSource =>
            {
                audioSource.Stop();
                audioSource.enabled = false;
                audioSource.gameObject.name = "Pooled AudioSource";
            },
            audioSource => Destroy(audioSource)
        );
    }

    public void StopMusic()
    {
        musicPlayer.Stop();
    }

    public void PlayMusic(AudioClip clip, bool loop = true)
    {
        if (clip == null) return;
        musicPlayer.clip = clip;
        musicPlayer.loop = loop;
        musicPlayer.Play();
    }

    public void PlayMusic(MusicId audioId, bool loop = true)
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

    private IEnumerator PlaySfx(AudioClip clip, float delay = 0, bool loop = false)
    {
        yield return new WaitForSeconds(delay);
        if (clip == null) yield break;
        var sfxPlayer = sfxPlayerPool.Get();
        sfxPlayer.gameObject.name = $"SFX: {clip.name}";
        if (loop)
        {
            sfxPlayer.loop = true;
            sfxPlayer.clip = clip;
            sfxPlayer.Play();
            loopingSfx.Add(sfxPlayer);
            yield break;
        }

        sfxPlayer.loop = false;
        sfxPlayer.PlayOneShot(clip);
        yield return new WaitForSeconds(clip.length);
        sfxPlayerPool.Release(sfxPlayer);
    }
    public void PlaySfxDelayed(SfxId audioId, float delay)
    {
        if (!sfxLookup.ContainsKey(audioId)) return;

        var audioData = sfxLookup[audioId];
        StartCoroutine(PlaySfx(audioData.clip, delay, false));
    }

    public void StopSfx()
    {
        foreach (var sfxPlayer in loopingSfx)
        {
            sfxPlayerPool.Release(sfxPlayer);
        }
        loopingSfx.Clear();
    }

    public void PlaySfx(SfxId audioId, bool loop = false)
    {
        if (!sfxLookup.TryGetValue(audioId, out var audioData)) return;
        Debug.Log($"Playing SFX {audioId} with clip name {audioData.clip.name}");
        StartCoroutine(PlaySfx(audioData.clip, 0, loop));
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
public class MusicData : AudioData
{
    public MusicId id;
}
[System.Serializable]
public class AudioData
{
    public AudioClip clip;
}
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;
using static UnityEngine.Rendering.DebugUI;
public enum SoundType
{
    Music,
    SFX
}
public enum AudioName
{
    Jump,
    Walk,
    Die,
    Switch,
    CameraSwitch,
    ItemSound,
}
public enum SnapShotName
{
    Water,
    Normal,
    Cave
}
[Serializable]
public class AudioGroup
{
    public AudioName name;
    public List<AudioClip> clips = new List<AudioClip>();
}

public class AudioManager : MonoBehaviour
{
    private static AudioManager _instance;
    public static AudioManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindAnyObjectByType<AudioManager>();
                if (_instance == null)
                    Debug.LogError("AudioManager가 없음");
            }
            return _instance;
        }
    }
    public AudioMixer audioMixer;
    public AudioSource Music;
    private float _musicVolume = 0.5f;
    public AudioSource SFX;
    private float _SFXVolume = 0.5f;
    public List<AudioGroup> audioGroups = new List<AudioGroup>();
    public List<AudioMixerSnapshot> snapshots;
    private Dictionary<string, AudioMixerSnapshot> snapShotDic = new Dictionary<string, AudioMixerSnapshot>();
    private readonly Dictionary<AudioName, List<AudioClip>> clipDic
        = new Dictionary<AudioName, List<AudioClip>>();

    public float MusicVolume
    {
        get 
        { 
            return _musicVolume;
        }
        set
        {
            _musicVolume = value;
            if (_musicVolume <= 0.0001f)
            {
                audioMixer.SetFloat("Music", -80f); // 완전 음소거
            }
            audioMixer.SetFloat("Music", Mathf.Log10(_musicVolume) * 20);
        }
    }

    public float SFXVolume
    {
        get
        {
            return _SFXVolume;
        }
        set
        {
            _SFXVolume = value;
            if (_SFXVolume <= 0.0001f)
            {
                audioMixer.SetFloat("SFX", -80f); // 완전 음소거
            }
            else
                audioMixer.SetFloat("SFX", Mathf.Log10(_SFXVolume) * 20);
        }
    }

    private void Awake()
    {
        InitializeAudioGroups();
        foreach (AudioMixerSnapshot snapshot in snapshots)
        {
            if(snapshot ==null) continue;
            snapShotDic.Add(snapshot.name,snapshot);
        }
    }

    private void InitializeAudioGroups()
    {
        clipDic.Clear();
        if (audioGroups == null)
            return;

        foreach (AudioGroup group in audioGroups)
        {
            if (group == null)
                continue;

            if (clipDic.ContainsKey(group.name))
            {
                Debug.LogWarning($"Duplicate audio group: {group.name}");
                continue;
            }

            var validClips = new List<AudioClip>();
            if (group.clips != null)
            {
                foreach (AudioClip clip in group.clips)
                {
                    if (clip != null)
                        validClips.Add(clip);
                }
            }
            clipDic.Add(group.name, validClips);
        }
    }

    private AudioClip GetRandomClip(AudioName name)
    {
        if (!clipDic.TryGetValue(name, out var candidates) || candidates.Count == 0)
            return null;

        int index = UnityEngine.Random.Range(0, candidates.Count);
        return candidates[index];
    }

    public void PlaySFXAudio(AudioName name)
    {
        AudioClip clip = GetRandomClip(name);
        if (clip == null)
            return;

        SFX.clip = clip;
        SFX.Play();
    }

    public void PlayOneShotSFXAudio(AudioName name)
    {
        AudioClip clip = GetRandomClip(name);
        if (clip == null)
            return;

        SFX.PlayOneShot(clip);
    }
    public void ChangeSnapShot(SnapShotName name)
    {
        snapShotDic[name.ToString()].TransitionTo(2.0f);
    }
}

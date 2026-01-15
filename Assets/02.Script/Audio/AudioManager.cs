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
    jump1,
    jump2,
    jump3,
    Walk,
    Die,
    Switch,
    CameraSwitch,
}
public enum SnapShotName
{
    Water,
    Normal,
    Cave
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
    public List<AudioClip> clips;
    public List<AudioMixerSnapshot> snapshots;
    private Dictionary<string, AudioMixerSnapshot> snapShotDic = new Dictionary<string, AudioMixerSnapshot>();
    private Dictionary<string, AudioClip> clipDic = new Dictionary<string, AudioClip>();

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
        foreach (AudioClip clip in clips) 
        {
            if (clip == null) continue;
            clipDic.Add(clip.name, clip);
        }
        foreach (AudioMixerSnapshot snapshot in snapshots)
        {
            if(snapshot ==null) continue;
            snapShotDic.Add(snapshot.name,snapshot);
        }
    }

    public void PlaySFXAudio(AudioName name)
    {
        SFX.clip = clipDic[name.ToString()];
        SFX.Play();
    }
    public void PlayOneShotSFXAudio(AudioName name)
    {
        SFX.PlayOneShot(clipDic[name.ToString()]);
    }
    public void ChangeSnapShot(SnapShotName name)
    {
        snapShotDic[name.ToString()].TransitionTo(2.0f);
    }
}

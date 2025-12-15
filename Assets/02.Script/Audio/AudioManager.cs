using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public enum AudioName
{
    jump1,
    jump2,
    jump3
}
public enum SnapShotName
{
    Water,
    Normal,

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
                    Debug.LogError("AudioManager°¡ ¾øÀ½");
            }
            return _instance;
        }
    }
    public AudioMixer audioMixer;
    public AudioSource Music;
    public AudioSource SFX;
    public List<AudioClip> clips;
    public List<AudioMixerSnapshot> snapshots;
    private Dictionary<string, AudioMixerSnapshot> snapShotDic = new Dictionary<string, AudioMixerSnapshot>();
    private Dictionary<string, AudioClip> clipDic = new Dictionary<string, AudioClip>();

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
            Debug.Log(snapshot.name);
            snapShotDic.Add(snapshot.name,snapshot);
        }
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            PlaySFXAudio(AudioName.jump1);
        }
        if (Input.GetKeyDown(KeyCode.N))
        {
            PlaySFXAudio(AudioName.jump2);
        }

        if (Input.GetKeyDown(KeyCode.G))
        {
            ChangeSnapShot(SnapShotName.Normal);
        }
        if (Input.GetKeyDown(KeyCode.H))
        {
            ChangeSnapShot(SnapShotName.Water);
        }
    }
    public void PlaySFXAudio(AudioName name)
    {
        SFX.PlayOneShot(clipDic[name.ToString()]);
    }
    public void ChangeSnapShot(SnapShotName name)
    {
        snapShotDic[name.ToString()].TransitionTo(2.0f);
    }
}

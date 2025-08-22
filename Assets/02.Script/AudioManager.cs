using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class Sound
{
    public string name;

    public AudioClip clip;
    public AudioSource source;

    public void SetSource(AudioSource _source)
    {
        source = _source;
        source.clip = clip;
    }
    public void Play()
    {
        source.Play();
    }
}

public class AudioManager : MonoBehaviour
{
    //[SerializeField]
    public Sound[] sounds;
    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i<sounds.Length; i++)
        {
            GameObject soundsource = new GameObject("sound name : " +  sounds[i].name);
            sounds[i].SetSource(soundsource.AddComponent<AudioSource>());
        }
    }

    public void Play(string _name)
    {
        for(int i = 0; i<sounds.Length; i++)
        {
            if(_name == sounds[i].name)
            {
                //sounds[i].source.volume = GameManager.instance.soundVolume; 추후 추가 변수명
                sounds[i].Play();
                return;
            }
        }
    }

}

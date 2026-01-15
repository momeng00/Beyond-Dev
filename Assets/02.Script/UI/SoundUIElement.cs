using CarouselUI;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundUIElement : CarouselUIElement
{
    public Slider slider;
    public List<Image> images;
    private float _volume;
    public SoundType soundType;
    private void Start()
    {
        switch (soundType)
        {
            case SoundType.Music:
                _volume = AudioManager.Instance.MusicVolume;
                AudioManager.Instance.MusicVolume = _volume;
                break;
            case SoundType.SFX:
                _volume = AudioManager.Instance.SFXVolume;
                AudioManager.Instance.SFXVolume = _volume;
                break;
        }
    }
    public float Volume
    {
        get
        {
            return _volume;
        }
        set
        {
            _volume = Mathf.Clamp(value, 0, 1);
            switch (soundType)
            {
                case SoundType.Music:
                    AudioManager.Instance.MusicVolume = _volume;
                    break;
                case SoundType.SFX:
                    AudioManager.Instance.SFXVolume = _volume;
                    break;
            }
        }

    }



    public override void PressNext()
    {
        Volume += 0.1f;
        slider.value = Volume;
    }
    
    public override void PressPrevious()
    {
        Volume -= 0.1f;
        slider.value = Volume;
    }
    public override void Selected()
    {
        Color color = image.color;
        color.a = 1f;
        image.color = color;
        foreach (var image in images)
        {
            color = image.color;
            color.a = 1f;
            image.color = color;
        }
        
    }
    public override void UnSelected()
    {
        Color color = image.color;
        color.a = 0.3f;
        image.color = color;
        foreach (var image in images)
        {
            color = image.color;
            color.a = 0.3f;
            image.color = color;
        }
    }
}

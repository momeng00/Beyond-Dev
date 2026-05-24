using CarouselUI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
[Serializable]
public enum selectState
{
    select,
    unselect,
}
public class SoundUIElement : CarouselUIElement
{
    [Serializable]
    public struct customSelectImage
    {
        public selectState state;
        public Sprite sprite;
    }
    public Image targetImage; //바뀔 이미지
    [SerializeField]public List<customSelectImage> images = new List<customSelectImage>(); //선택대상마다 바뀌는 이미지를 집어넣는 곳
    public Slider slider;
    private float _volume;
    public SoundType soundType;
    private void Awake()
    {
        UnSelected();
    }
    private void Start()
    {
        switch (soundType)
        {
            case SoundType.Music:
                _volume = AudioManager.Instance.MusicVolume;
                AudioManager.Instance.MusicVolume = _volume;
                slider.value = _volume;
                break;
            case SoundType.SFX:
                _volume = AudioManager.Instance.SFXVolume;
                AudioManager.Instance.SFXVolume = _volume;
                slider.value = _volume;
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
    public void ChangeCustomImage(selectState state)
    {
        foreach (var image in images)
        {
            if(image.state == state)
            {
                targetImage.sprite = image.sprite;
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
        //Color color = image.color;
        //color.a = 1f;
        //image.color = color;
        ChangeCustomImage(selectState.select);
        //foreach (var image in images)
        //{
        //    color = image.color;
        //    color.a = 1f;
        //    image.color = color;
        //}

    }
    public override void UnSelected()
    {
        //Color color = image.color;
        //color.a = 0.3f;
        //image.color = color;
        ChangeCustomImage(selectState.unselect);
        //foreach (var image in images)
        //{
        //    color = image.color;
        //    color.a = 0.3f;
        //    image.color = color;
        //}
    }
    IEnumerator CSelectAnimation()
    {
        yield return null;
    }
}

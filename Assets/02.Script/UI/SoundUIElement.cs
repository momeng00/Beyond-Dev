using CarouselUI;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundUIElement : CarouselUIElement
{
    public Slider slider;
    public List<Image> images;
    private float _volume = 0.5f;
    
    public float Volume
    {
        get
        {
            return _volume;
        }
        set
        {
            _volume = Mathf.Clamp(value, 0, 1);
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
        color.a = 0.5f;
        image.color = color;
        foreach (var image in images)
        {
            color = image.color;
            color.a = 0.5f;
            image.color = color;
        }
    }
}

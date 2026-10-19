using CarouselUI;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static SoundUIElement;

public class OptionUIElement : CarouselUIElement
{
    public Image leftImage;
    [SerializeField] public List<customSelectImage> leftImages = new List<customSelectImage>(); //선택대상마다 바뀌는 이미지를 집어넣는 곳
    public Image rightImage;
    [SerializeField] public List<customSelectImage> rightImages = new List<customSelectImage>(); //선택대상마다 바뀌는 이미지를 집어넣는 곳
    private void Awake()
    {
        UnSelected();
    }
    public void ChangeCustomImage(selectState state)
    {
        foreach (var image in leftImages)
        {
            if (image.state == state)
            {
                leftImage.sprite = image.sprite;
            }
        }
        foreach (var image in rightImages)
        {
            if (image.state == state)
            {
                rightImage.sprite = image.sprite;
            }
        }
    }
    public override void Selected()
    {
        PlaySelectionAnimation(FadeSelectionImage(image, 1f, 0.2f));
        ChangeCustomImage(selectState.select);
    }

    public override void UnSelected()
    {
        PlaySelectionAnimation(FadeSelectionImage(image, 0f, 0.2f));
        ChangeCustomImage(selectState.unselect);
    }
}

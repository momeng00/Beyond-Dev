using CarouselUI;
using UnityEngine;
using UnityEngine.UI;

public class SwitchUIElement : CarouselUIElement
{
    public Image leftImage;
    public Image rightImage;
    private Image currentImage;
    private void Start()
    {
        //기본이 키보드(left위치에 있음) << 설정이 패드가 아닌 키보드라는 뜻
        //나중에 설정을 로드하는 기능을 만들면 거기서 가져와서 설정을 시키는 방식으로 변경해야함
        currentImage = leftImage;
        Color color = currentImage.color;
        color.a = 1f;
        currentImage.color = color;
    }
    public override void PressNext()
    {
        if(currentImage == leftImage)
        {
            SwitchUnSelected();
            currentImage = rightImage;
            SwitchSelected();
            
        }
    }
    public override void PressPrevious()
    {
        if (currentImage == rightImage)
        {
            SwitchUnSelected();
            currentImage = leftImage;
            SwitchSelected();

        }
    }
    public void SwitchSelected()
    {
        Color color = currentImage.color;
        color.a = 1f;
        currentImage.color = color;
        //current가 바뀐것에 따라서 설정바뀌게 하는 코드 여기에
    }

    public void SwitchUnSelected()
    {
        Color color = currentImage.color;
        color.a = 0f;
        currentImage.color = color;
    }
}
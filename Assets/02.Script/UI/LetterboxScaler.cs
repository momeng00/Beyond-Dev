using UnityEngine;


public enum ScreenSize
{
    Small,      //960*540
    Medium,     //1280*720
    Large       //1920*1080
}
public class LetterboxScaler : MonoBehaviour
{
    //화면 비율을 따로 조정할 수 없도록 만들기 때문에 레터박스는 자동으로 여기에 종속됨
    public Transform upLetterbox;
    public Transform downLetterbox;
    private int width;
    private int height;
    private ScreenSize currentScreen = ScreenSize.Large;
    private FullScreenMode currentScreenMode = FullScreenMode.FullScreenWindow;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.J))
        {
            SetScaler(ScreenSize.Small);
        }
        if (Input.GetKeyDown(KeyCode.K))
        {
            SetScaler(ScreenSize.Medium);
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            SetScaler(ScreenSize.Large);
        }
        if (Input.GetKeyDown(KeyCode.B))
        {
            SetScreenMode(FullScreenMode.ExclusiveFullScreen);
        }
        if (Input.GetKeyDown(KeyCode.N))
        {
            SetScreenMode(FullScreenMode.FullScreenWindow);
        }
        if (Input.GetKeyDown(KeyCode.M))
        {
            SetScreenMode(FullScreenMode.Windowed);
        }
    }


    public void SetScaler(ScreenSize size)
    {
        switch (size)
        {
            case ScreenSize.Small:
                currentScreen = ScreenSize.Small;
                width = 960;
                height = 540;
                break;
            case ScreenSize.Medium:
                currentScreen = ScreenSize.Medium;
                width = 1280;
                height = 720;
                break;
            case ScreenSize.Large:
                currentScreen = ScreenSize.Large;
                width = 1920;
                height = 1080;
                break;

        }
        Screen.SetResolution(width, height, currentScreenMode);
    }
    public void SetScreenMode(FullScreenMode mode)
    {
        switch (mode)
        {
            case FullScreenMode.ExclusiveFullScreen:
                currentScreenMode = FullScreenMode.ExclusiveFullScreen;
                break;
            case FullScreenMode.FullScreenWindow:
                currentScreenMode = FullScreenMode.FullScreenWindow;
                break;
            case FullScreenMode.Windowed:
                currentScreenMode= FullScreenMode.Windowed;
                break;
        }
        Screen.SetResolution(width, height, currentScreenMode);
    }
}
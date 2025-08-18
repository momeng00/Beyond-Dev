using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class GameReadyProtocol : MonoBehaviour
{
    public Language language;
    public int gameSound;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            Screen.SetResolution(1920, 1080, FullScreenMode.ExclusiveFullScreen);
        }
        if (Input.GetKeyDown(KeyCode.F2))
        {
            Screen.SetResolution(1920, 1080, FullScreenMode.FullScreenWindow);
        }
        if (Input.GetKeyDown(KeyCode.F3))
        {
            Screen.SetResolution(1920, 1080, FullScreenMode.Windowed);
        }
    }
}

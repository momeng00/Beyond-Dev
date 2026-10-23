using UnityEngine;

public class UI_PauseMenu : UIWindow
{
    public UIWindow settingMenu;
    public UIWindow quitSign;
    public UIWindow _quitSignInstance;
    public void OpenSettingMenu()
    {

    }
    public void OpenQuitSign()
    {
        if (quitSign != null)
        {
            if (_quitSignInstance == null)
            {
                _quitSignInstance = Instantiate(quitSign); ;
                _quitSignInstance.name = quitSign.name;
#if UNITY_EDITOR
                Debug.Log("창 생성");
#endif
            }
            _quitSignInstance.Open();
#if UNITY_EDITOR
            Debug.Log("창 오픈");
#endif
        }
    }
}

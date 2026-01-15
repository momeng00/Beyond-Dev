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
                Debug.Log("창 생성");
            }
            _quitSignInstance.Open();
            Debug.Log("창 오픈");
        }
    }
}

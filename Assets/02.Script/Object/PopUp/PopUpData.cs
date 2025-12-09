using System.Collections.Generic;

[System.Serializable]
public class PopUpData
{
    public string key;
    public string name_kr;
    public string name_en;    
    public string content_kr;
    public string content_en;
    public string profileID;
    public string profileImage;
    public string Name
    {
        get
        {
            if (LanguageSystem.Instance.currentLanguage == Language.Korean)
            {
                return name_kr;
            }
            else
            {
                return name_en;
            }
        }

    }
    public string Content
    {
        get
        {
            if (LanguageSystem.Instance.currentLanguage == Language.Korean)
            {
                return content_kr;
            }
            else
            {
                return content_en;
            }
        }

    }
}
[System.Serializable]
public class PopupDataTable
{
    public List<PopUpData> items;
}


[System.Serializable]
public struct SmartKey
{
    // 실제 데이터는 이 Enum 하나뿐입니다.
    public PopupKey key;
}
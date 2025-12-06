public class PopUpData
{

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
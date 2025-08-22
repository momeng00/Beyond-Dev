using TMPro;
using UnityEngine;

public class TextSetTemp : MonoBehaviour
{
    private TMP_Text text;
    public LocalizationKeys localizationKey;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        text = GetComponent<TMP_Text>();
        text.text = LanguageSystem.Instance.GetText(localizationKey.ToString());
        LanguageSystem.OnLanguageChanged += ChangeLanguage;
    }

    public void ChangeLanguage()
    {
        if(text == null)
        {
            text = GetComponent<TMP_Text>();
        }
        text.text = LanguageSystem.Instance.GetText(localizationKey.ToString());
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}

using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

public class MessageBlock : Block, ISwitchable
{
    public string checktext;
    //글씨를 넣기위한 기능으로 변경예정
    // --- 인스펙터 설정 변수 ---
    public LocalizationKeys localizationKey;
    [Header("Text Margins & Alignment")]
    [Tooltip("텍스트의 사방 여백(Margins)을 조절합니다. (좌, 상, 우, 하)")]
    public Vector4 textMargins; // TextMeshPro의 Margin 속성을 직접 제어

    [Tooltip("텍스트의 정렬 위치를 선택합니다.")]
    public TextAlignmentOptions alignment = TextAlignmentOptions.Center;

    [Header("Multi-Language Content")]
    [Tooltip("각 언어에 맞는 텍스트를 입력합니다.")]
    public List<LanguageEntry> languageEntries = new List<LanguageEntry>();

    // --- 내부 변수 ---
    private RectTransform textRectTransform;
    private Dictionary<Language, string> languageDictionary;

    // 인스펙터에서 언어와 텍스트를 한 쌍으로 입력받기 위한 구조체
    [System.Serializable]
    public struct LanguageEntry
    {
        public Language language;
        public string text;
    }

    public TextMeshPro text;
    public GameObject emoji;
    public int fontSize;
    private BoxCollider2D boxCollider;
    public Switch Switch => throw new System.NotImplementedException();

    public void SwitchOn(bool value)
    {
        throw new System.NotImplementedException();
    }
    private void Awake()
    { 

        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        Sprite sprite = spriteRenderer.sprite;
        boxCollider = GetComponent<BoxCollider2D>();

        Vector2 boundsWithoutScale = new Vector2(
            spriteRenderer.bounds.size.x / transform.localScale.x,
            spriteRenderer.bounds.size.y / transform.localScale.y
        );
        TextMeshPro tmp = GetComponentInChildren<TextMeshPro>();
        if (tmp == null)
        {
            GameObject textObj = new GameObject("Text");

            // 위치 초기화
            textObj.transform.localRotation = Quaternion.identity;
            textObj.transform.localScale = Vector3.one;
            textObj.transform.localPosition = transform.position;
            // TextMeshPro 컴포넌트 추가
            tmp = textObj.AddComponent<TextMeshPro>();
            textObj.transform.SetParent(transform);
        }
        else 
        {
            tmp.gameObject.transform.localRotation = Quaternion.identity;
            tmp.gameObject.transform.localScale = new Vector3(
                1f / transform.localScale.x,
                1f / transform.localScale.y,
                1f / transform.localScale.z
            );
            tmp.gameObject.transform.localPosition = transform.localPosition;
        }

        tmp.sortingOrder = spriteRenderer.sortingOrder;

        // 텍스트 설정

        tmp.text = LanguageSystem.Instance.GetText(localizationKey.ToString());
        checktext = tmp.text;

        tmp.fontSize = this.fontSize;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.enableAutoSizing = false;
        tmp.color = Color.black;
        text = tmp;
        if (boxCollider == null)
        {
            boxCollider = gameObject.AddComponent<BoxCollider2D>();
        }
        boxCollider.size = boundsWithoutScale;

        RectTransform rectTransform = tmp.rectTransform;
        Vector2 spriteWorldSize = spriteRenderer.bounds.size;

        Vector2 localSize = rectTransform.InverseTransformVector(spriteWorldSize);
        rectTransform.sizeDelta = localSize;

        Initialize();
        UpdateTextObject();
        
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void Start()
    {
        base.Start();
        LanguageSystem.OnLanguageChanged += ChangeLanguage;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void ChangeLanguage()
    {
        if (text == null)
        {
            text = gameObject.GetComponentInChildren<TextMeshPro>();
        }
        text.text = LanguageSystem.Instance.GetText(localizationKey.ToString());
    }
    /// <summary>
    /// 컴포넌트 참조 및 딕셔너리 초기화
    /// </summary>
    private void Initialize()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        text = text.GetComponent<TextMeshPro>();
        textRectTransform = text.GetComponent<RectTransform>();

        // List를 Dictionary로 변환하여 런타임 시 빠른 텍스트 조회를 준비
        languageDictionary = new Dictionary<Language, string>();
        foreach (var entry in languageEntries)
        {
            if (!languageDictionary.ContainsKey(entry.language))
            {
                languageDictionary.Add(entry.language, entry.text);
            }
        }
    }

    /// <summary>
    /// 텍스트 오브젝트의 모든 속성을 업데이트하는 메인 함수
    /// </summary>
    public void UpdateTextObject()
    {
        if (boxCollider == null || text == null) return;

        // 1. 콜라이더 크기에 맞춰 RectTransform 크기 설정 (패딩 없음)
        //textRectTransform.sizeDelta = boxCollider.size;

        // 2. TextMeshPro 자체의 Margin 기능으로 패딩 적용
        text.margin = textMargins;
        
        // 3. 텍스트 정렬 설정
        text.alignment = alignment;
        text.textWrappingMode = TextWrappingModes.NoWrap;
        text.text = LanguageSystem.Instance.GetText(localizationKey.ToString());
    }
}


public enum Language
{
    Korean,
    English,
    Japanese
}

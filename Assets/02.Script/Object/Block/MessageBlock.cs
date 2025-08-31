using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

public class MessageBlock : Block, ISwitchable
{
    //글씨를 넣기위한 기능으로 변경예정
    // --- 인스펙터 설정 변수 ---

    [Header("Text Margins & Alignment")]
    [Tooltip("텍스트의 사방 여백(Margins)을 조절합니다. (좌, 상, 우, 하)")]
    public Vector4 textMargins; // TextMeshPro의 Margin 속성을 직접 제어

    [Tooltip("텍스트의 정렬 위치를 선택합니다.")]
    public TextAlignmentOptions alignment = TextAlignmentOptions.Center;

    [Header("Multi-Language Content")]
    [Tooltip("각 언어에 맞는 텍스트를 입력합니다.")]
    public List<LanguageEntry> languageEntries = new List<LanguageEntry>();

    // --- 내부 변수 ---
    private TextMeshPro textMeshPro;
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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void Start()
    {
        base.Start();
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        Sprite sprite = spriteRenderer.sprite;

        Vector2 boundsWithoutScale = new Vector2(
            spriteRenderer.bounds.size.x / transform.localScale.x,
            spriteRenderer.bounds.size.y / transform.localScale.y
        );

        GameObject textObj = new GameObject("Text");

        // 위치 초기화
        textObj.transform.localRotation = Quaternion.identity;
        textObj.transform.localScale = Vector3.one;
        textObj.transform.localPosition = transform.position;
        // TextMeshPro 컴포넌트 추가
        TextMeshPro tmp = textObj.AddComponent<TextMeshPro>();
        text = tmp;
        // 텍스트 설정
        tmp.text = "Hello World!";
        
        tmp.fontSize = this.fontSize;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.enableAutoSizing = false;
        tmp.color = Color.black;
        if (boxCollider == null) 
        {
            boxCollider = gameObject.AddComponent<BoxCollider2D>();
        }
        boxCollider.size = boundsWithoutScale;

        RectTransform rectTransform = tmp.rectTransform;
        Vector2 spriteWorldSize = spriteRenderer.bounds.size;

        Vector2 localSize = rectTransform.InverseTransformVector(spriteWorldSize);
        rectTransform.sizeDelta = localSize;

        textObj.transform.SetParent(transform);
        
        Initialize();
        UpdateTextObject();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// 컴포넌트 참조 및 딕셔너리 초기화
    /// </summary>
    private void Initialize()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        textMeshPro = text.GetComponent<TextMeshPro>();
        textRectTransform = textMeshPro.GetComponent<RectTransform>();

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
        if (boxCollider == null || textMeshPro == null) return;

        // 1. 콜라이더 크기에 맞춰 RectTransform 크기 설정 (패딩 없음)
        //textRectTransform.sizeDelta = boxCollider.size;

        // 2. TextMeshPro 자체의 Margin 기능으로 패딩 적용
        textMeshPro.margin = textMargins;

        // 3. 텍스트 정렬 설정
        textMeshPro.alignment = alignment;

        // 4. GameManager에서 현재 설정된 언어를 가져와 텍스트 업데이트
        // TODO: 아래 GameManager 참조는 임시로 사용된 변수입니다.
        //Language currentLanguage = GameManager.Instance.currentLanguage;
        Language currentLanguage = Language.Korean;

        if (languageDictionary.ContainsKey(currentLanguage))
        {
            textMeshPro.text = languageDictionary[currentLanguage];
        }
        else
        {
            // 해당하는 언어의 텍스트가 없으면 경고 메시지 표시
            textMeshPro.text = "N/A";
            Debug.LogWarning($"'{gameObject.name}' 오브젝트에 '{currentLanguage}' 언어에 대한 텍스트가 없습니다.", this);
        }
    }
}


public enum Language
{
    Korean,
    English,
    Japanese
}

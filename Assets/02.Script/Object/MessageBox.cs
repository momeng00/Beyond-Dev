using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MessageBox : MonoBehaviour
{
    //메세지를 입력하는 방식 메세지가 들어가 있을 곳은 따로 있어서 그곳에 있는 데이터를 이용해서 데이터를 입력할 예정
    //메세지 블럭에 입력하는 이유는 collider.bound.size를 이용해서 크기를 지정하는 것을 이용해서 할 것이기 때문임.
    //하위 자식에 text가 이미 존재한다면 해당 text의 글씨만 변경하는 것을 이용할 것이고, 없다면 위를 이용해서 크기와 폰트를 설정
    //text가 켜지고 끄는 타이밍을 조절하는 것은 어디서 작동되야할까? Block이 switch를 연결할때 그걸 이용해서 할 수 있나?
    //스위치는 시작하자마자 자기자신을 내가 가지고 있는 List안에 있는 Switch들에게 등록을 하는 것이고
    //이 스위치들을 가져와서 나도 등록하면? 근데 Switch의 값을 가져오는 것이 아니라 BlockState를 변경하는 방식이 존재해서 문제가 생김
    //BlockState를 가져와서 하는게 가장 편한데 OnStateChange라는 기능을 추가해서 하는 것이 좋을까?
    //위의 방식이 가장 편할것같긴 할 텐데. BlockState라는 곳에 의존되어서 진행되는 것이 맞을까요?
    //하위 오브젝트에 text가 있는지 체크 -> 없으면 만들어서 할당(이떄) -> 설정받은 텍스트를 입력-> 
    //*이때 부모 오브젝트에서 Block을 가져와서 collider의 크기에 맞춰서 크기를 할당해줘야하는데?
    //만들어야할 기능 (크기를 가져와서 그 크기에 맞춰서 text사이즈를 지정하는 것. 폰트 크기도)
    //만들어야할 기능 (LocalizationKeys를 이용해서 텍스트를 변경하는 것 및 Language가 변경되면 Update)
    //OnChange(Block에 있는 거)에 기능을 등록시키는 것

    // --- 인스펙터 설정 변수 ---
    public LocalizationKeys localizationKey;

    [Tooltip("텍스트의 정렬 위치를 선택합니다.")]
    public TextAlignmentOptions alignment = TextAlignmentOptions.Center;
    public int fontSize;
    // --- 내부 변수 ---
    private RectTransform textRectTransform;
    private Dictionary<Language, string> languageDictionary;
    private Animator animator;
    [SerializeField]private TextMeshPro text;

    private void Awake()
    {
        Initialize();
        //FindTextObject();
        if(animator == null)
        {
            animator = text.GetComponent<Animator>();
        }
        gameObject.GetComponent<Block>().blockEvent += BlockStateHandle;    
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        //UpdateTextObject();
        //LanguageSystem.OnLanguageChanged += ChangeLanguage;
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
        languageDictionary = new Dictionary<Language, string>();
    }

    /// <summary>
    /// 텍스트 오브젝트의 모든 속성을 업데이트하는 메인 함수
    /// </summary>
    public void UpdateTextObject()
    {
        if (text == null) return;
        text.text = LanguageSystem.Instance.GetText(localizationKey.ToString());
    }
    public void BlockStateHandle(bool state)
    {
        animator.SetBool("IsActive", state);
    }
    private void FindTextObject()
    {
        if (text == null)
            text = GetComponentInChildren<TextMeshPro>();
        if (text == null)
        {
            GameObject prefabToLoad = Resources.Load<GameObject>("Text");
            GameObject textObj =Instantiate(prefabToLoad, Vector3.zero, Quaternion.identity);
            Animator newAnimator = textObj.GetComponent<Animator>();
            // 위치 초기화
            textObj.transform.localRotation = Quaternion.identity;
            textObj.transform.localScale = Vector3.one;
            textObj.transform.localPosition = transform.position;
            text = textObj.GetComponent<TextMeshPro>();
            textObj.transform.SetParent(transform);
            text.fontSize = fontSize;
            text.alignment = alignment;
            text.enableAutoSizing = false;
            text.sortingOrder = gameObject.GetComponent<SpriteRenderer>().sortingOrder + 1;
            text.rectTransform.sizeDelta = gameObject.GetComponent<BoxCollider2D>().size;
            animator = newAnimator;
        }
        text.text = LanguageSystem.Instance.GetText(localizationKey.ToString());
    }
}




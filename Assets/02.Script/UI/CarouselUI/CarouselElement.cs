using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CarouselElement : UIBase
{
    [Header ("OnOpen, OnClose 안됨")] //event로 선언해놓았기때문
    private Image image;
    //private RectTransform rectTransform; 이미 UIBase에 존재함
    //private CanvasGroup canvasGroup; 이미 UIBase에 존재함
    private Coroutine _moveCoroutine;
    public float enterTime = 2.2f;
    public float aniEnterTime = 0.6f;
    private Animator ani;
    public bool flag=false; //테스트용 체크 표시 나중에 삭제
    public string enterAnimationName;
    public string exitAnimationName;
    public float todoEnterDelay=0.4f;
    public TodoList todoList;
    protected override void Awake()
    {
        base.Awake();
        openAnimation = UIAnimType.SlideRight;
        ani = GetComponent<Animator>();
        image = GetComponent<Image>();
    }

    protected override void Start()
    {
        base.Start();
        CarouselUIManager.Instance.AddCarousel(this);   
    }
    private void Update()
    {
        if (flag)
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                Open();
            }
            if (Input.GetKeyDown(KeyCode.W))
            {
                Close();
            }
        }
    }

    public override void Open()
    {
        Debug.Log("진입");
        gameObject.SetActive(true);
        canvasGroup.alpha = 1f;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
        if (currentAnimationCoroutine != null) StopCoroutine(currentAnimationCoroutine);
        currentAnimationCoroutine = StartCoroutine(PlayOpenAnimation());
    }
    public override void Close()
    {
        canvasGroup.alpha = 1f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
        if (currentAnimationCoroutine != null) StopCoroutine(currentAnimationCoroutine);
        if (!gameObject.activeSelf)
            return;
        currentAnimationCoroutine = StartCoroutine(PlayCloseAnimation());
    }
    protected new IEnumerator PlayOpenAnimation()
    {
        // [핵심] 1프레임 대기 -> 유니티가 UI 정렬할 시간을 줌
        yield return null;

        // 시작 위치 계산
        float width = Screen.width;
        float height = Screen.height;
        Vector2 startPos = originalPosition;

        switch (openAnimation)
        {
            case UIAnimType.SlideLeft: startPos = new Vector2(-width, originalPosition.y); break;
            case UIAnimType.SlideRight: startPos = new Vector2(width + rectTransform.rect.width, originalPosition.y); break;
            case UIAnimType.SlideTop: startPos = new Vector2(originalPosition.x, height); break;
            case UIAnimType.SlideBottom: startPos = new Vector2(originalPosition.x, -height); break;
        }

        // 초기 위치 강제 적용
        if (openAnimation != UIAnimType.PopUp && openAnimation != UIAnimType.None)
            rectTransform.anchoredPosition = startPos;

        if (openAnimation == UIAnimType.PopUp)
            rectTransform.localScale = Vector3.zero;

        // 애니메이션 루프
        float timer = 0f;
        bool aniFlag = false;
        while (timer < enterTime)
        {
            float t;
            timer += Time.unscaledDeltaTime;
            t = Mathf.Sin((timer / enterTime) * Mathf.PI * 0.5f);
            if(aniEnterTime < timer && aniFlag == false)
            {
                ani.Play(enterAnimationName, 0, 0f);
                aniFlag = true;
            }
            // 1. 움직임/크기 변화
            if (openAnimation == UIAnimType.PopUp)
                rectTransform.localScale = Vector3.Lerp(Vector3.zero, originalScale, t);
            else if (openAnimation != UIAnimType.None)
                rectTransform.anchoredPosition = Vector2.Lerp(startPos, originalPosition, t);

            // 2. [추가됨] 투명도: 0(안보임) -> 1(잘보임) 로 서서히 변화
            //canvasGroup.alpha = Mathf.Lerp(0f, 1f, t);

            yield return null;
        }

        // 끝난 후 확실하게 값 고정
        rectTransform.anchoredPosition = originalPosition;
        rectTransform.localScale = originalScale;
        todoList.Open();
        canvasGroup.alpha = 1f; // 완전히 보이게
    }

    protected new IEnumerator PlayCloseAnimation()
    {
        float timer = 0f;
        Vector2 startPos = rectTransform.anchoredPosition;
        Vector3 startScale = rectTransform.localScale;

        // 목표 지점 계산
        Vector2 targetPos = originalPosition;
        Vector3 targetScale = originalScale;
        float width = Screen.width;
        float height = Screen.height;

        switch (openAnimation)
        {
            case UIAnimType.PopUp: targetScale = Vector3.zero; break;
            case UIAnimType.SlideLeft: targetPos = new Vector2(width, originalPosition.y); break;
            case UIAnimType.SlideRight: targetPos = new Vector2(-(width + rectTransform.rect.width), originalPosition.y); break;
            case UIAnimType.SlideTop: targetPos = new Vector2(originalPosition.x, -height); break;
            case UIAnimType.SlideBottom: targetPos = new Vector2(originalPosition.x, height); break;
        }
        todoList.Close();
        while (timer < enterTime)
        {
            timer += Time.unscaledDeltaTime;
            
            float t = Mathf.Pow(Mathf.Sin((timer / enterTime) * Mathf.PI * 0.5f), 0.5f);
            // 1. 움직임/크기 변화
            if (openAnimation == UIAnimType.PopUp)
                rectTransform.localScale = Vector3.Lerp(startScale, targetScale, t);
            else if (openAnimation != UIAnimType.None)
                rectTransform.anchoredPosition = Vector2.Lerp(startPos, targetPos, t);

            // 2. [추가됨] 투명도: 1(잘보임) -> 0(안보임) 로 서서히 변화
            //canvasGroup.alpha = Mathf.Lerp(1f, 0f, t);

            yield return null;
        }

        //// 끝난 후 끄기
        //canvasGroup.alpha = 0f; // 완전히 투명하게
        //gameObject.SetActive(false);

        // 다음 오픈을 위해 원상복구
        //rectTransform.anchoredPosition = originalPosition;
        //rectTransform.localScale = originalScale;
    }
    //기본이 될 그려질 순서를 정하기
    public void SetSortingOrder(int index)
    {
        image.transform.SetSiblingIndex(index);
        init();
    }

    //들어오고 나서 TodoList를 완료시키기 위한 기능
    public void OverList()
    {

    }

    //요소들의 위치를 초기화하기 위한 기능
    public void init()
    {

    }
    public void AddMoveTo()
    {
        StartCoroutine(AddAnimation());
    }
    public void MoveTo(Vector2 targetX,float size,float rot, float duration)
    {
        if (_moveCoroutine != null) StopCoroutine(_moveCoroutine);
        _moveCoroutine = StartCoroutine(MoveCoroutine(targetX, duration,rot,size));
    }

    private IEnumerator MoveCoroutine(Vector2 targetX, float duration,float rot,float size)
    {
        float startX = rectTransform.anchoredPosition.x;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            

            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0, 1, elapsed / duration);  // 부드러운 이동
            float currentRot = Mathf.Lerp(rectTransform.localRotation.y, rot, t);
            float currentSize = Mathf.Lerp(rectTransform.localScale.y, size, t);

            rectTransform.localRotation = Quaternion.Euler(0f, currentRot, 0f);
            rectTransform.localScale = Vector3.one * currentSize;
            rectTransform.anchoredPosition = new Vector2(Mathf.Lerp(startX, targetX.x, t),
                                                  rectTransform.anchoredPosition.y);
            yield return null;
        }

        rectTransform.anchoredPosition = new Vector2(targetX.x, rectTransform.anchoredPosition.y);
        rectTransform.localRotation = Quaternion.Euler(0f, rot, 0f);
        rectTransform.localScale = new Vector3(size, size, size);
    }
    IEnumerator AddAnimation(float duration = 0.56f)
    {
        float time = 0f;

        Vector2 startPos = new Vector2(260f, rectTransform.anchoredPosition.y);
        Vector2 endPos = new Vector2(0f, rectTransform.anchoredPosition.y);

        rectTransform.anchoredPosition = startPos;
        canvasGroup.alpha = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;

            // 부드러운 ease (중요)
            t = Mathf.SmoothStep(0f, 1f, t);

            rectTransform.anchoredPosition = Vector2.Lerp(startPos, endPos, t);
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, t);

            yield return null;
        }

        // 마지막 보정
        rectTransform.anchoredPosition = endPos;
        canvasGroup.alpha = 1f;
    }

    //선택된 요소가 중앙에 있으면서 새로 들어오는 요소가 추가되는 작동방식이 필요함.
    //init로 가능한가? 위치를 Manager가 있는 곳이 기준점이 되고 좌로 이동시키면?
    //위치를 지정하는 방식이 뭐가 있을까?
    //0,10을 기준점으로 하면 될것같은데 선택된 요소를 기준점으로 두고 x의 값을 변경시키는 방식으로 진행하고
    //이동 자체를 어떤식으로 해야할까? 코루틴?
    //Update문으로도 가능하기는 한테 입력을 받는것도 아니고 다른 방식으로 구현하고 싶은데?
    //
}
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
public enum UIAnimType { None, PopUp, SlideLeft, SlideRight, SlideTop, SlideBottom, OriginPopUp }
// CanvasGroup과 RectTransform은 애니메이션을 위해 필수
[RequireComponent(typeof(CanvasGroup), typeof(RectTransform))]
public class UIBase : MonoBehaviour
{
    [Header("Animation Settings")]
    public UIAnimType openAnimation = UIAnimType.PopUp;
    public float animDuration = 0.3f;

    // 애니메이션 이벤트 (필요하면 쓰세요)
    public event Action onOpen;
    public event Action onClose;

    protected CanvasGroup canvasGroup;
    protected RectTransform rectTransform;
    protected Vector2 originalPosition;
    protected Vector3 originalScale;

    protected virtual void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        rectTransform = GetComponent<RectTransform>();

        // 초기 위치 저장
        originalPosition = rectTransform.anchoredPosition;
        originalScale = rectTransform.localScale;
    }

    protected virtual void Start()
    {

    }

    // ------------------------------------------------
    // [핵심 변경] Open/Close는 이제 애니메이션만 신경 씁니다.
    // Manager 등록 로직은 자식 클래스(UIWindow)로 넘어갔습니다.
    // ------------------------------------------------
    protected Coroutine currentAnimationCoroutine;
    public virtual void Open()
    {
        //gameObject.SetActive(true);
        canvasGroup.alpha = 0f;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
        onOpen?.Invoke();
        if (currentAnimationCoroutine != null) StopCoroutine(currentAnimationCoroutine);
        currentAnimationCoroutine = StartCoroutine(PlayOpenAnimation());
    }
    
    public virtual void Close()
    {
        canvasGroup.alpha = 1f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
        onClose?.Invoke();
        if (currentAnimationCoroutine != null) StopCoroutine(currentAnimationCoroutine);
        //if (!gameObject.activeSelf)    
        //    return;
        currentAnimationCoroutine = StartCoroutine(PlayCloseAnimation());
    }


    public void SetInitialState()
    {
        // 이제 Open 코루틴 안에서 초기화를 다 하므로, 
        // 외부에서 강제로 초기화할 때만 이 함수를 쓰면 됩니다.
        //gameObject.SetActive(true);
        if (openAnimation == UIAnimType.PopUp) rectTransform.localScale = Vector3.zero;
        canvasGroup.alpha = 0f; // 안 보이게
    }

    // =========================================================
    // 애니메이션 코루틴 (Alpha 페이드 추가됨)
    // =========================================================
    protected IEnumerator PlayOpenAnimation()
    {
        // [핵심] 1프레임 대기 -> 유니티가 UI 정렬할 시간을 줌
        yield return null;

        // 1프레임 뒤, 정렬된 진짜 위치를 저장
        //originalPosition = rectTransform.anchoredPosition;
        //originalScale = Vector3.one;

        // 시작 위치 계산
        float width = Screen.width;
        float height = Screen.height;
        Vector2 startPos = originalPosition;

        switch (openAnimation)
        {
            case UIAnimType.SlideLeft: startPos = new Vector2(-width, originalPosition.y); break;
            case UIAnimType.SlideRight: startPos = new Vector2(width, originalPosition.y); break;
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
        while (timer < animDuration)
        {
            timer += Time.unscaledDeltaTime;
            float t = Mathf.Sin((timer / animDuration) * Mathf.PI * 0.5f);

            // 1. 움직임/크기 변화
            if (openAnimation == UIAnimType.PopUp)
                rectTransform.localScale = Vector3.Lerp(Vector3.zero, originalScale, t);
            else if (openAnimation != UIAnimType.None)
                rectTransform.anchoredPosition = Vector2.Lerp(startPos, originalPosition, t);

            // 2. [추가됨] 투명도: 0(안보임) -> 1(잘보임) 로 서서히 변화
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, t);

            yield return null;
        }

        // 끝난 후 확실하게 값 고정
        rectTransform.anchoredPosition = originalPosition;
        rectTransform.localScale = originalScale;
        canvasGroup.alpha = 1f; // 완전히 보이게
    }

    

    protected IEnumerator PlayCloseAnimation()
    { 
        yield return null;
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
            case UIAnimType.SlideLeft: targetPos = new Vector2(-width, originalPosition.y); break;
            case UIAnimType.SlideRight: targetPos = new Vector2(width, originalPosition.y); break;
            case UIAnimType.SlideTop: targetPos = new Vector2(originalPosition.x, height); break;
            case UIAnimType.SlideBottom: targetPos = new Vector2(originalPosition.x, -height); break;
        }

        while (timer < animDuration)
        {
            timer += Time.unscaledDeltaTime;
            float t = Mathf.Sin((timer / animDuration) * Mathf.PI * 0.5f);

            // 1. 움직임/크기 변화
            if (openAnimation == UIAnimType.PopUp)
                rectTransform.localScale = Vector3.Lerp(startScale, targetScale, t);
            else if (openAnimation != UIAnimType.None)
                rectTransform.anchoredPosition = Vector2.Lerp(startPos, targetPos, t);

            // 2. [추가됨] 투명도: 1(잘보임) -> 0(안보임) 로 서서히 변화
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, t);

            yield return null;
        }

        // 끝난 후 끄기
        canvasGroup.alpha = 0f; // 완전히 투명하게
        //gameObject.SetActive(false);

        // 다음 오픈을 위해 원상복구
        rectTransform.anchoredPosition = originalPosition;
        rectTransform.localScale = originalScale;
        Debug.Log("끝남");
    }
}
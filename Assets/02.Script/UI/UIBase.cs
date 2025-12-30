using System;
using System.Collections;
using UnityEngine;
public enum UIAnimType { None, PopUp, SlideLeft, SlideRight, SlideTop, SlideBottom }
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

    public virtual void Open()
    {
        gameObject.SetActive(true);
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;

        onOpen?.Invoke();
        StopAllCoroutines();
        StartCoroutine(PlayOpenAnimation());
    }

    public virtual void Close()
    {
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        onClose?.Invoke();
        StopAllCoroutines();
        StartCoroutine(PlayCloseAnimation());
    }


    public void SetInitialState()
    {
        // 1. 코루틴이 돌려면 오브젝트가 켜져 있어야 함 (문제 2번 해결)
        gameObject.SetActive(true);

        // 2. 인터랙션 차단 (대기 중일 때 클릭 방지)
        if (canvasGroup != null)
        {
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
            // (선택) 대기 중에 깜빡임 방지를 위해 투명하게 할 수도 있음
            // canvasGroup.alpha = 1f; // 혹은 0f
        }

        // 3. 시작 위치/크기 계산 (PlayOpenAnimation과 동일한 로직)
        float width = Screen.width;
        float height = Screen.height;

        // 만약 originalPosition이 아직 세팅 안 됐다면(Awake 전이라면) 강제 세팅
        if (rectTransform == null) rectTransform = GetComponent<RectTransform>();

        // 주의: Awake가 실행되기 전일 수 있으므로 안전장치
        if (originalPosition == Vector2.zero && rectTransform != null)
            originalPosition = rectTransform.anchoredPosition;
        if (originalScale == Vector3.zero && rectTransform != null)
            originalScale = rectTransform.localScale;

        switch (openAnimation)
        {
            case UIAnimType.PopUp:
                rectTransform.localScale = Vector3.zero;
                break;
            case UIAnimType.SlideLeft:
                rectTransform.anchoredPosition = new Vector2(-width, originalPosition.y);
                break;
            case UIAnimType.SlideRight:
                rectTransform.anchoredPosition = new Vector2(width, originalPosition.y);
                break;
            case UIAnimType.SlideTop:
                rectTransform.anchoredPosition = new Vector2(originalPosition.x, height);
                break;
            case UIAnimType.SlideBottom:
                rectTransform.anchoredPosition = new Vector2(originalPosition.x, -height);
                break;
        }
    }
    // --- 애니메이션 로직 (동일함) ---
    protected IEnumerator PlayOpenAnimation()
    {
        float timer = 0f;
        float width = Screen.width;
        float height = Screen.height;

        switch (openAnimation)
        {
            case UIAnimType.PopUp: rectTransform.localScale = Vector3.zero; break;
            case UIAnimType.SlideLeft: rectTransform.anchoredPosition = new Vector2(-width, originalPosition.y); break;
            case UIAnimType.SlideRight: rectTransform.anchoredPosition = new Vector2(width, originalPosition.y); break;
            case UIAnimType.SlideTop: rectTransform.anchoredPosition = new Vector2(originalPosition.x, height); break;
            case UIAnimType.SlideBottom: rectTransform.anchoredPosition = new Vector2(originalPosition.x, -height); break;
        }

        Vector2 currentStartPos = rectTransform.anchoredPosition;

        while (timer < animDuration)
        {
            timer += Time.unscaledDeltaTime;
            float t = Mathf.Sin((timer / animDuration) * Mathf.PI * 0.5f);

            if (openAnimation == UIAnimType.PopUp)
                rectTransform.localScale = Vector3.Lerp(Vector3.zero, originalScale, t);
            else if (openAnimation != UIAnimType.None)
                rectTransform.anchoredPosition = Vector2.Lerp(currentStartPos, originalPosition, t);
            yield return null;
        }
        rectTransform.anchoredPosition = originalPosition;
        rectTransform.localScale = originalScale;
    }

    protected IEnumerator PlayCloseAnimation()
    {
        float timer = 0f;
        Vector2 startPos = rectTransform.anchoredPosition;
        Vector3 startScale = rectTransform.localScale;

        // 닫힐 목표 지점 계산
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

            if (openAnimation == UIAnimType.PopUp)
                rectTransform.localScale = Vector3.Lerp(startScale, targetScale, t);
            else if (openAnimation != UIAnimType.None)
                rectTransform.anchoredPosition = Vector2.Lerp(startPos, targetPos, t);
            yield return null;
        }
        gameObject.SetActive(false);
        rectTransform.anchoredPosition = originalPosition;
        rectTransform.localScale = originalScale;
    }
}
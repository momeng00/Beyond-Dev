using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TodoList : MonoBehaviour
{
    protected Vector2 originalPosition;
    protected Coroutine currentAnimationCoroutine;
    protected RectTransform rectTransform;
    public float TodoDelay = 0.2f;
    public List<Todo> todoes;
    CanvasGroup cg;
    private void Awake()
    {
        cg = GetComponent<CanvasGroup>();
        cg.alpha = 0f;
        rectTransform = GetComponent<RectTransform>();
        originalPosition = rectTransform.anchoredPosition;
        gameObject.SetActive(false);
    }
    
    public void Open()
    {
        gameObject.SetActive(true);
        cg.alpha = 0f;
        if (currentAnimationCoroutine != null) StopCoroutine(currentAnimationCoroutine);
        currentAnimationCoroutine = StartCoroutine(PlayOpenAnimation());
    }
    public void Close()
    {
        foreach (var todo in todoes)
        {
            todo.Close();
        }
        if (currentAnimationCoroutine != null) StopCoroutine(currentAnimationCoroutine);
        if (!gameObject.activeSelf)
            return;
        currentAnimationCoroutine = StartCoroutine(PlayCloseAnimation());
    }
    protected IEnumerator PlayOpenAnimation()
    {
        // [핵심] 1프레임 대기 -> 유니티가 UI 정렬할 시간을 줌
        yield return null;

        // 시작 위치 계산
        float width = Screen.width;
        float height = Screen.height;
        Vector2 startPos = originalPosition;
        rectTransform.anchoredPosition = startPos;
        cg.alpha = 1f;
        startPos = new Vector2(originalPosition.x, height);

        // 애니메이션 루프
        float timer = 0f;
        while (timer < 1.2f)
        {
            float t;
            timer += Time.unscaledDeltaTime;
            t = Mathf.Sin((timer / 1.2f) * Mathf.PI * 0.5f);

            // 1. 움직임/크기 변화
            rectTransform.anchoredPosition = Vector2.Lerp(startPos, originalPosition, t);

            // 2. [추가됨] 투명도: 0(안보임) -> 1(잘보임) 로 서서히 변화
            //canvasGroup.alpha = Mathf.Lerp(0f, 1f, t);

            yield return null;
        }
        foreach (Todo todo in todoes)
        {
            todo.CheckClear();
            yield return new WaitForSeconds(TodoDelay);
        }
        // 끝난 후 확실하게 값 고정
        rectTransform.anchoredPosition = originalPosition;
    }

    protected IEnumerator PlayCloseAnimation()
    {
        float timer = 0f;
        Vector2 startPos = rectTransform.anchoredPosition;
        Vector3 startScale = rectTransform.localScale;

        // 목표 지점 계산
        Vector2 targetPos = originalPosition;

        float width = Screen.width;
        float height = Screen.height;

        targetPos = new Vector2(originalPosition.x, height);

        while (timer < 1.2f)
        {
            timer += Time.unscaledDeltaTime;

            float t = Mathf.Pow(Mathf.Sin((timer / 1.2f) * Mathf.PI * 0.5f), 0.5f);
            // 1. 움직임/크기 변화
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
}

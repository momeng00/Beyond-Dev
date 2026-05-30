using System.Collections;
using TMPro;
using UnityEngine;

public class InGameTodo : MonoBehaviour
{
    public TMP_Text text;
    protected Vector2 originalPosition;
    protected Coroutine currentAnimationCoroutine;
    protected RectTransform rectTransform;
    public float TodoDelay = 0.72f;
    private Animator ani;
    private CanvasGroup cg;
    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        originalPosition = rectTransform.anchoredPosition;
        cg = GetComponent<CanvasGroup>();
        ani = GetComponent<Animator>();
    }
    private void Update()
    {
        if (Input.anyKey)
        {
            BannerNotice("");
        }
    }
    public void BannerNotice(string text)
    {
        if(this.text != null)
        {
            this.text.text = text;
        }
        currentAnimationCoroutine = StartCoroutine(PlayOpenAnimation());
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
        while (timer < TodoDelay)
        {
            float t;
            timer += Time.unscaledDeltaTime;
            t = Mathf.Sin((timer / TodoDelay) * Mathf.PI * 0.5f);

            // 1. 움직임 변화
            rectTransform.anchoredPosition = Vector2.Lerp(startPos, originalPosition, t);

            // 2. [추가됨] 투명도: 0(안보임) -> 1(잘보임) 로 서서히 변화
            //canvasGroup.alpha = Mathf.Lerp(0f, 1f, t);

            yield return null;
        }
        // 끝난 후 확실하게 값 고정
        rectTransform.anchoredPosition = originalPosition;
        ani.Play("Todo_Done");
        yield return new WaitForSeconds(1);
        OutAnimation();
    }
    public void OutAnimation()
    {
        currentAnimationCoroutine = StartCoroutine(PlayCloseAnimation());
    }
    protected IEnumerator PlayCloseAnimation()
    {
        // [핵심] 1프레임 대기 -> 유니티가 UI 정렬할 시간을 줌
        yield return null;

        // 시작 위치 계산
        float width = Screen.width;
        float height = Screen.height;
        Vector2 startPos = rectTransform.anchoredPosition;

        cg.alpha = 1f;
        Vector2 targetPos = new Vector2(originalPosition.x, height);

        // 애니메이션 루프
        float timer = 0f;
        while (timer < TodoDelay)
        {
            float t;
            timer += Time.unscaledDeltaTime;
            t = 1f - Mathf.Cos((timer / TodoDelay) * Mathf.PI * 0.5f);

            // 1. 움직임 변화
            rectTransform.anchoredPosition = Vector2.Lerp(startPos, targetPos, t);

            // 2. [추가됨] 투명도: 0(안보임) -> 1(잘보임) 로 서서히 변화
            //canvasGroup.alpha = Mathf.Lerp(0f, 1f, t);

            yield return null;
        }
        // 끝난 후 확실하게 값 고정
        rectTransform.anchoredPosition = targetPos;
        ani.Play("Todo_Default");
    }
}
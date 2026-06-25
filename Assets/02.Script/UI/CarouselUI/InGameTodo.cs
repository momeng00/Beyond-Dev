using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static UnityEngine.Audio.ProcessorInstance;
public struct BannerData
{
    public string text;

    public BannerData(string text)
    {
        this.text = text;
    }
}
public class InGameTodo : MonoBehaviour
{
    //인게임에서 진행되는 Todo들을 관리하기 위한 시스템 ITEM과 연동되어서 작동하겠금 제작되었음
    //여러개를 한번에 요구하게 된다면 Queue를 통해서 한번씩 나오도록 할까?
    public TMP_Text text;
    protected Vector2 originalPosition;
    protected Coroutine currentAnimationCoroutine;
    protected RectTransform rectTransform;
    public float TodoInDelay = 0.72f;
    public float TodoOutDelay = 0.36f;
    private Animator ani;
    private CanvasGroup cg;
    private bool isWorking = false;
    private Queue<BannerData> responses = new Queue<BannerData>(); 
    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        originalPosition = rectTransform.anchoredPosition;
        cg = GetComponent<CanvasGroup>();
        ani = GetComponent<Animator>();
        responses.Clear();
        OutAnimation();
    }
    private void Update()
    {
        if (responses.Count>0 && !isWorking)
        {
            isWorking = true;
            ShowBanner(responses.Dequeue());
        }
    }
    public void BannerNotice(string text)
    {
        //아이콘이나 호출할때는 이걸이용해서 호출 아이콘용 스프라이트나 이런걸 초기화 하려면 여기에 추가
        if(this.text != null)
        {
            responses.Enqueue(new BannerData(text));
        }
        
    }
    private void ShowBanner(BannerData data)
    {
        isWorking = true;

        text.text = data.text;
        if(currentAnimationCoroutine!=null)
            StopCoroutine(currentAnimationCoroutine);
        currentAnimationCoroutine =
            StartCoroutine(PlayOpenAnimation());
    }
    protected IEnumerator PlayOpenAnimation()
    {
        // 1프레임 대기 -> 유니티가 UI 정렬할 시간을 줌
        yield return null;

        // 시작 위치 계산
        float width = Screen.width;
        float height = Screen.height;
        Vector2 startPos;
        startPos = new Vector2(originalPosition.x, height + rectTransform.rect.height);
        rectTransform.anchoredPosition = startPos;
        cg.alpha = 0f;
        

        // 애니메이션 루프
        float timer = 0f;
        while (timer < TodoInDelay)
        {
            float t;
            timer += Time.unscaledDeltaTime;
            t = Mathf.Sin((timer / TodoInDelay) * Mathf.PI * 0.5f);
            cg.alpha = Mathf.Lerp(cg.alpha, 1f, t);
            // 1. 움직임 변화
            rectTransform.anchoredPosition = Vector2.Lerp(startPos, originalPosition, t);

            // 2. [추가됨] 투명도: 0(안보임) -> 1(잘보임) 로 서서히 변화
            //canvasGroup.alpha = Mathf.Lerp(0f, 1f, t);

            yield return null;
        }
        // 끝난 후 확실하게 값 고정
        rectTransform.anchoredPosition = originalPosition;
        cg.alpha = 1f;
        ani.Play("Todo_Done");
        yield return new WaitForSeconds(1.44f);
        OutAnimation();
    }
    public void OutAnimation() //해당 스크립트는 애니메이션 이벤트를 이용해서 관리할것 타이밍을 맞출것. << 이거 안됨.
    {
        if(currentAnimationCoroutine != null)
        {
            StopCoroutine(currentAnimationCoroutine);
        }
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
        Vector2 targetPos = new Vector2(originalPosition.x, height+rectTransform.rect.height);

        // 애니메이션 루프
        float timer = 0f;
        while (timer < TodoOutDelay)
        {
            float t;
            timer += Time.unscaledDeltaTime;
            t = 1f - Mathf.Cos((timer / TodoOutDelay) * Mathf.PI * 0.5f);

            // 1. 움직임 변화
            rectTransform.anchoredPosition = Vector2.Lerp(startPos, targetPos, t);
            cg.alpha = Mathf.Lerp(cg.alpha, 0f, t);
            // 2. [추가됨] 투명도: 0(안보임) -> 1(잘보임) 로 서서히 변화
            //canvasGroup.alpha = Mathf.Lerp(0f, 1f, t);

            yield return null;
        }
        // 끝난 후 확실하게 값 고정
        cg.alpha = 0f;
        rectTransform.anchoredPosition = targetPos;
        ani.Play("Todo_Default");
        isWorking = false;
    }
}
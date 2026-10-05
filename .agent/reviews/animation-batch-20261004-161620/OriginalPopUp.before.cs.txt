using System.Collections;
using TMPro;
using UnityEngine;
using static UnityEngine.UI.Image;

public class OriginalPopUp : NormalPopUp
{
    //Canvas가 없으며 RectTransform을 가지고 있는 기준으로 만들어 진거입니다.
    //Canvas가 없으면 CanvasGroup이 실행이 안됨.
    //팝업이 BlockStage를 따라감, ToggleEvent가 실행되면 BlockState에 따라서 팝업이 나오고 안나오고 임.
    protected CanvasGroup canvasGroup;
    protected RectTransform rectTransform;
    protected Vector2 originalPosition;
    protected Vector3 originalScale;
    private Coroutine currentCoroutine;
    public float animDuration;
    private Animator animator;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        animator = GetComponent<Animator>();
        // 초기 위치 저장
        originalPosition = rectTransform.anchoredPosition;
        originalScale = rectTransform.localScale;
        FindChildAnimator();
        ToggleEvent(false);
        //InitPopUpDatas(); 지금은 데이터 테이블이 없어서 주석처리 해둠. 오류문뜨는거 싫어서
    }
    private void Start()
    {
        GameManager.Instance.OnReset += ResetAction;
    }

    public override void ToggleEvent(bool state, Transform origin = null)
    {
        base.ToggleEvent(state, origin);
        if (state)
        {
            Open(origin);
        }
        else if (!state)
        {
            Close();
        }
    }
    
    public virtual void Open(Transform original)
    {
        if (original == null)
            return;

        //호출한 위치에서 부터 원본 위치까지 이동하는 스크립트

        gameObject.SetActive(true);
        //canvasGroup.interactable = true;
        //canvasGroup.blocksRaycasts = true; 클릭이 필요없기에 항상 false로

        if (currentCoroutine != null) StopCoroutine(currentCoroutine);
        currentCoroutine = StartCoroutine(PlayOpenAnimation(original));
    }
    protected IEnumerator PlayOpenAnimation(Transform original)
    {

        //nickName.enabled = false;
        //content.enabled = false;
        //profileID.enabled = false;
        // [핵심] 1프레임 대기 -> 유니티가 UI 정렬할 시간을 줌
        yield return null;

        // 시작 위치 계산
        float width = Screen.width;
        float height = Screen.height;
        Vector2 startPos = originalPosition;

        startPos = original.position;
        rectTransform.localScale = Vector3.zero;

        // 애니메이션 루프
        float timer = 0f;
        while (timer < animDuration)
        {
            timer += Time.unscaledDeltaTime;
            float t = Mathf.Sin((timer / animDuration) * Mathf.PI * 0.5f);

            //움직임/크기 변화
            rectTransform.localScale = Vector3.Lerp(Vector3.zero, originalScale, t);
            rectTransform.anchoredPosition = Vector2.Lerp(startPos, originalPosition, t);

            //투명도: 0(안보임) -> 1(잘보임) 로 서서히 변화
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, t);
            //if (timer < animDuration * 2 && nickName.enabled == false)
            //{
            //    nickName.enabled = true;
            //    content.enabled = true;
            //    profileID.enabled = true;
            //} 위치값이 이상해지는 버그가있음 중간에 꺼서 그런듯
            yield return null;
        }

        // 끝난 후 확실하게 값 고정
        rectTransform.anchoredPosition = originalPosition;
        rectTransform.localScale = originalScale;
        canvasGroup.alpha = 1f; // 완전히 보이게
    }
    public virtual void Close()
    {
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
        if (currentCoroutine != null) StopCoroutine(currentCoroutine);
        if (!gameObject.activeSelf)
            return;
        currentCoroutine = StartCoroutine(PlayCloseAnimation());
    }
    protected IEnumerator PlayCloseAnimation()
    {
        float timer = 0f;
        Vector3 startScale = rectTransform.localScale;
        Vector3 targetScale;
        float width = Screen.width;
        float height = Screen.height;
        targetScale = Vector3.zero;


        while (timer < animDuration)
        {
            timer += Time.unscaledDeltaTime;
            float t = Mathf.Sin((timer / animDuration) * Mathf.PI * 0.5f);
            // 크기, 투명도 변화
            rectTransform.localScale = Vector3.Lerp(startScale, targetScale, t);
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, t);
            yield return null;
        }
        //nickName.enabled = false;
        //content.enabled = false;
        //profileID.enabled = false;
        // 끝난 후 끄기
        //canvasGroup.alpha = 0f; // 완전히 투명하게 캔버스가 canvas안에 있어야지만 적용
        // 다음 오픈을 위해 원상복구
        //rectTransform.anchoredPosition = originalPosition;
        //rectTransform.localScale = originalScale;
        //gameObject.SetActive(false);
    }
}
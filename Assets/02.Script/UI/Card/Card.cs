using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct ItemInTodo
{
    public Todo todo;
    public ClearCondition item;
}
[RequireComponent(typeof(CanvasGroup))]
public class Card : MonoBehaviour
{
    //애니메이션이나 키고 꺼짐의 [기능!]을 만들어야함. 기능을 사용하는 것은 Manager에 종속되어야함.
    //고양이카드를 관리하는 스크립트
    //카드는 Manager가 Instance로 싱글톤이니 이를 최대한 이용해보자
    public List<ItemInTodo> itemInTodoes; // 이건 아이템이랑 연결되는 Todo들임
    [HideInInspector]public List<Todo> todoList; //이건 그냥 투두들을 넣을 공간임
    public Action<Card> OnTodoAnimationFinished; //여기에 카드 집어넣는 기능 넣기
    private int completedTodoCount;
    public void TodoAnimationFinished()
    {
        completedTodoCount++;

        if (completedTodoCount >= itemInTodoes.Count)
        {
            OnTodoAnimationFinished?.Invoke(this);
            Debug.Log("카드 엔딩 실행됨");
        }
    }
    
    public float todoDelay=0.36f; //다음 todo가 체크되는 딜레이
    private CanvasGroup canvasGroup;
    public CanvasGroup CanvasGroup => canvasGroup;
    protected RectTransform rectTransform;
    public RectTransform Rect => rectTransform;
    public Vector2 OriginalPosition { get; private set; }
    public Vector3 OriginalScale { get; private set; }
    private Coroutine coroutine;
    protected Animator animator;
    public Action Done;

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            CardManager.Instance.AddCard(this);
        }
    }
    private void Awake()
    {
        //병신아 Clone을 만들때는 Start문이 사용이 안되다는 걸 명심해.
        //아니였음. Awake문은 생성직후 Start문은 생성이 된 뒤 업데이트가 실행되기 직전에 실행이 되므로
        //생성한뒤 바로 AddCard를 실행하니 생성- awake문 - AddCard-Start문 - Update문으로 실행이 된거임
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        animator = GetComponent<Animator>();

        OriginalPosition = Rect.anchoredPosition;
        OriginalScale = Rect.localScale;
    }
    private void Start()
    {
        InitCard();
        RemoveCard();
    }
    public void InitCard()
    {
        completedTodoCount = 0;
        foreach (var item in itemInTodoes)
        {
            if (item.todo != null)
            {
                item.todo.SubscribeTodoAction(TodoAnimationFinished);
                Debug.Log("구독들어감");
            }
            else
                Debug.LogError("todo가 없음.");

            if (item.item != null)
                item.item.AddOnCheck(item.todo.DoSuccess);
        }
    }
    public void RemoveCard()
    {
        canvasGroup.alpha = 0f;
    }

    public void SubscriptCardEnd(Action<Card> action)
    {
        OnTodoAnimationFinished += action;
    }
    public void InitTodoList()
    {

    }
    public void PlayAnimation(string animationName)
    {
        animator.Play(animationName);
    }
    public void MoveCard(int index, float ratio, float animDuration)
    {
        animator.Play("In");
        //gameObject.GetComponent<UIBaseUpgrade>().Open(targetPos, originalScale); //안이뻐서 제거
        //레이어 순서는 미리미리 지정해놔야함.
        StartCoroutine(MoveCardCoroutine(index, ratio, animDuration));
    }
    public void SuccessClearStage()
    {
        //해당 todoList안에 있는 content를 체크해서 isClear 및 체크 및 InGameTodo를 띄워줄 수 있도록 한다. (Todo에게 이전함)
        //안에 든 모든 Todo를 완료시키는 스크립트가 존재해야함.
        //이거는 카드안에 있는 애니메이션을 통해서 타이밍을 조절한다.
        //카드안에 있는 애니메이션에 이 메서드를 집어넣는다.
        coroutine = StartCoroutine(CheckCondition());

    }
    public void NextStep()
    {
        //EffectManager.instance.BlurAnimation(true);
        RemoveCard();
    }
    public void StartGameCardOpen()
    {
        animator.Play("In_Start_ForSys");
    }
    public void CheckClearCard()
    {
        canvasGroup.alpha = 1f;
        animator.Play("In_Start");
    }
    IEnumerator CheckCondition()
    {
        foreach (var item in itemInTodoes)
        {
            item.todo.CheckClear();
            yield return new WaitForSeconds(todoDelay);
        }
    }
    IEnumerator MoveCardCoroutine(int index, float ratio, float animDuration)
    {
        canvasGroup.alpha = 1f;
        yield return null;

        float targetOffsetX = rectTransform.rect.x * (ratio / 100) * (index - 1);
        // ratio(0~100)를 0~1로 정규화해서 목표 X 오프셋 계산


        Vector2 startPos = rectTransform.anchoredPosition;
        Vector2 targetPos = OriginalPosition + new Vector2(targetOffsetX, 0f); // 왼쪽으로 이동
        //Vector3 targetScale = originalScale * (ratio / (100f + (10f * index)));

        float elapsed = 0f;
        float t;
        while (elapsed < animDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            t = Mathf.Sin((elapsed / animDuration) * Mathf.PI * 0.5f);
            //rectTransform.localScale = Vector3.Lerp(rectTransform.localScale, targetScale, t);
            rectTransform.anchoredPosition = Vector2.Lerp(startPos, targetPos, t);
            yield return null;
        }
        //rectTransform.localScale = targetScale;
        rectTransform.anchoredPosition = targetPos; // 오차 보정
    }
    public IEnumerator CCardRemove()
    {
        yield return null;
        float elapsed = 0f;
        float t;
        while (elapsed < 0.3f)
        {
            elapsed += Time.unscaledDeltaTime;
            t = Mathf.Sin((elapsed / 0.3f) * Mathf.PI * 0.5f);
            //rectTransform.localScale = Vector3.Lerp(rectTransform.localScale, targetScale, t);

            yield return null;
        }
    }
}
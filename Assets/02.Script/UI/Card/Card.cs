using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct ItemInTodo
{
    public ClearCondition item;
    public Todo todo;
}
[RequireComponent(typeof(CanvasGroup))]
public class Card : MonoBehaviour
{
    //고양이카드를 관리하는 스크립트
    public List<ItemInTodo> itemInTodoes;
    public float todoDelay=0.36f; //다음 todo가 체크되는 딜레이
    private CanvasGroup canvasGroup;
    private Coroutine coroutine;
    protected Vector2 originalPos; //다 똑같은 위치에 둬야함.
    protected Vector3 originalScale;
    protected RectTransform rectTransform;
    protected Animator animator;
    private void Awake()
    {
        //병신아 Clone을 만들때는 Start문이 사용이 안되다는 걸 명심해.
        //아니였음. Awake문은 생성직후 Start문은 생성이 된 뒤 업데이트가 실행되기 직전에 실행이 되므로
        //생성한뒤 바로 AddCard를 실행하니 생성- awake문 - AddCard-Start문 - Update문으로 실행이 된거임
        rectTransform = GetComponent<RectTransform>();
        originalPos = rectTransform.anchoredPosition;
        originalScale = rectTransform.localScale;
        canvasGroup = GetComponent<CanvasGroup>();
        animator = GetComponent<Animator>();
    }
    private void Start()
    {
        InitCard();
    }
    public void InitCard()
    {
        foreach (var item in itemInTodoes)
        {
            if(item.item !=null )
                item.item.AddOnCheck(item.todo.DoSuccess);
        }
    }
    public void RemoveCard()
    {
        canvasGroup.alpha = 0f;
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
        EffectManager.instance.BlurAnimation(true);
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
            yield return null;
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
        Vector2 targetPos = originalPos + new Vector2(targetOffsetX, 0f); // 왼쪽으로 이동
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
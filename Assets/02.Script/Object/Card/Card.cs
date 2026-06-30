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
[RequireComponent(typeof(CanvasGroup),typeof(UIBaseUpgrade))]
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
    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        originalPos = rectTransform.anchoredPosition;
        originalScale = rectTransform.localScale;
    }
    private void Start()
    {
        InitCard();
    }
    public void InitCard()
    {
        foreach (var item in itemInTodoes)
        {
            item.item.AddOnCheck(item.todo.DoSuccess);
        }
    }
    public void MoveCard(int index, float ratio, float animDuration)
    {
        Debug.Log($"{gameObject.name}원본 위치는 {originalPos}");
        //원본 크기의 ratio/100을 곱한 것 만큼 곱하기 count-index를 빼면 그 옆까지 이동을 한다.
        float ratioOffset = rectTransform.rect.x * (ratio/100) * index;
        Vector2 targetPos = originalPos - new Vector2(ratioOffset, 0f);
        rectTransform.anchoredPosition = targetPos;
        //gameObject.GetComponent<UIBaseUpgrade>().Open();
        //StartCoroutine(MoveCardCoroutine(index, ratio, animDuration));
    }
    public void SuccessClearStage()
    {
        //해당 todoList안에 있는 content를 체크해서 isClear 및 체크 및 InGameTodo를 띄워줄 수 있도록 한다. (Todo에게 이전함)
        //안에 든 모든 Todo를 완료시키는 스크립트가 존재해야함.
        //이거는 카드안에 있는 애니메이션을 통해서 타이밍을 조절한다.
        //카드안에 있는 애니메이션에 이 메서드를 집어넣는다.
        coroutine = StartCoroutine(CheckCondition());

    }
    IEnumerator CheckCondition()
    {

        foreach (var item in itemInTodoes)
        {
            item.todo.CheckClear();
            yield return new WaitForSeconds(todoDelay);
        }
        yield return null;
    }
    IEnumerator MoveCardCoroutine(int index, float ratio, float animDuration)
    {
        float maxOffsetX = rectTransform.rect.x;
        yield return null;
        float ratioOffset = 100f - ((index-1)*(100-ratio));

        // ratio(0~100)를 0~1로 정규화해서 목표 X 오프셋 계산
        float targetOffsetX = (ratioOffset / 100f) * maxOffsetX;

        Vector2 startPos = rectTransform.anchoredPosition;
        Vector2 targetPos = originalPos - new Vector2(targetOffsetX, 0f); // 왼쪽으로 이동
        //Vector3 targetScale = originalScale * (ratio / (100f + (10f * index)));

        float elapsed = 0f;

        while (elapsed < animDuration)
        {
            elapsed += Time.deltaTime;
            float t;
            elapsed += Time.unscaledDeltaTime;
            t = Mathf.Sin((elapsed / animDuration) * Mathf.PI * 0.5f);
            //rectTransform.localScale = Vector3.Lerp(rectTransform.localScale, targetScale, t);
            rectTransform.anchoredPosition = Vector2.Lerp(startPos, targetPos, t);
            yield return null;
        }
        //rectTransform.localScale = targetScale;
        rectTransform.anchoredPosition = targetPos; // 오차 보정
    }
}
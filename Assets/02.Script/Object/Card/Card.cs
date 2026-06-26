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
}
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
    [HideInInspector] public List<Todo> todoList; //이건 그냥 투두들을 넣을 공간임
    private int completedTodoCount;


    public Action<Card> OnTodoAnimationFinished; //여기에 카드 집어넣는 기능 넣기

    public float todoDelay = 0.36f; //다음 todo가 체크되는 딜레이
    #region 기본 선언
    protected Animator animator;
    private CanvasGroup canvasGroup;
    public CanvasGroup CanvasGroup => canvasGroup;
    protected RectTransform rectTransform;
    public RectTransform Rect => rectTransform;
    public Vector2 OriginalPosition { get; private set; }
    public Vector3 OriginalScale { get; private set; }
    #endregion
    private Coroutine coroutine;

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
        RemoveCard(this);
    }
    #region 초기화 기능 및 초기화에 넣을 기능
    public void InitCard()
    {
        completedTodoCount = 0;
        foreach (var item in itemInTodoes)
        {
            todoList.Add(item.todo);
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
    //여기에 뭐 넣을라고 했는지 체크해보기
    //여기안에 종료시키는 걸 넣을라고 했음
    public void SubscriptCardEnd(Action<Card> action)
    {
        Debug.Log("추가");
        OnTodoAnimationFinished += action;
    }


    //카드안에있는 모든 Todo가 완료되었는지 확인하기
    //Todo들에 있는 Action에 이걸 등록시켜서 애니메이션이 이걸 호출함
    public void TodoAnimationFinished()
    {
        completedTodoCount++;

        if (completedTodoCount >= itemInTodoes.Count)
        {
            RemoveCard(this);
            OnTodoAnimationFinished?.Invoke(this);
            Debug.Log("카드 엔딩 실행됨");
        }
    }
    #endregion

    public void PlayAnimation(string animationName)
    {
        animator.Play(animationName);
    }

    public void RemoveCard(Card card = null)
    {
        card.PlayAnimation("Default");
        foreach(Todo todo in todoList)
        {
            todo.Close();
        }
        canvasGroup.alpha = 0f;
        Debug.Log("카드종료");
    }

    //클리어 가 실행되면 실행될 애니메이션 중간에 타이밍을 조절해야함
    public void SuccessClearStage()
    {
        //해당 todoList안에 있는 content를 체크해서 isClear 및 체크 및 InGameTodo를 띄워줄 수 있도록 한다. (Todo에게 이전함)
        //안에 든 모든 Todo를 완료시키는 스크립트가 존재해야함.
        //이거는 카드안에 있는 애니메이션을 통해서 타이밍을 조절한다.
        //카드안에 있는 애니메이션에 이 메서드를 집어넣는다.
        coroutine = StartCoroutine(CheckCondition());

    }
    //Card 진입이 진행되면 실행될 메서드
    public void NextStep()
    {
        //EffectManager.instance.BlurAnimation(true);
        RemoveCard();
    }
    public void StartGameCardOpen()
    {
        PlayAnimation("In_Start_ForSys");
    }
    public void CheckClearCard()
    {
        canvasGroup.alpha = 1f;
        PlayAnimation("In_Clear");
    }
    #region 코루틴
    IEnumerator CheckCondition()
    {
        foreach (var item in itemInTodoes)
        {
            item.todo.CheckClear();
            yield return new WaitForSeconds(todoDelay);
        }
    }
    
    
    #endregion
}
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
public class Card : MonoBehaviour, IUI
{
    //애니메이션이나 키고 꺼짐의 [기능!]을 만들어야함. 기능을 사용하는 것은 Manager에 종속되어야함.
    //고양이카드를 관리하는 스크립트
    //카드는 Manager가 Instance로 싱글톤이니 이를 최대한 이용해보자
    public List<ItemInTodo> itemInTodoes; // 이건 아이템이랑 연결되는 Todo들임
    [HideInInspector] public List<Todo> todoList; //이건 그냥 투두들을 넣을 공간임
    private int completedTodoCount;


    public Action<Card> OnTodoAnimationFinished; //여기에 카드 집어넣는 기능 넣기
    public float animDuration = 3.2f;
    public float todoDelay = 0.36f; //다음 todo가 체크되는 딜레이

    public bool BeginEntrance(float duration)
    {
        ShowForLayout();
        if (!isActiveAndEnabled || !displayRequested) return false;
        canvasGroup.alpha = 0f;
        coroutine = StartCoroutine(FadeEntrance(duration));
        return true;
    }

    private IEnumerator FadeEntrance(float duration)
    {
        yield return null;
        yield return UIAnimationRoutine.Run(duration,
            progress => Mathf.Sin(progress * Mathf.PI * 0.5f),
            t => canvasGroup.alpha = t);
        coroutine = null;
        fadeFinished = true;
        TryFinishEntrance();
    }

    public void OnEntranceCompleted()
    {
        layoutFinished = true;
        TryFinishEntrance();
    }

    private void TryFinishEntrance()
    {
        if (displayState == DisplayState.Entering && layoutFinished && fadeFinished)
            SuccessClearStage();
    }

    private void OnTodoFinished(Todo todo)
    {
        if (displayState != DisplayState.Checking || !pendingTodos.Remove(todo)) return;
        completedTodoCount++;
        if (pendingTodos.Count == 0)
        {
            BeginWaitForInput();
            OnTodoAnimationFinished?.Invoke(this);
        }
    }

    private void BeginWaitForInput()
    {
        if (!displayRequested || (displayState != DisplayState.Entering &&
            displayState != DisplayState.Checking)) return;
        displayState = DisplayState.Waiting;
        readyFrame = Time.frameCount;
    }

    public void Show() => StartGameCardOpen();

    public void Hide()
    {
        if (inputActionEnabled && displayState == DisplayState.Waiting &&
            Time.frameCount > readyFrame)
            RemoveCard();
    }

    public void InputAction()
    {
        if (Input.anyKeyDown) Hide();
    }

    public void SetVisible(bool state)
    {
        if (canvasGroup == null) return;
        canvasGroup.interactable = state;
        canvasGroup.blocksRaycasts = state;
    }

    private void Update()
    {
        if (inputActionEnabled) InputAction();
    }

    private void FinishClose()
    {
        if (!displayRequested) return;
        displayRequested = false;
        displayState = DisplayState.Hidden;
        inputActionEnabled = false;
        pendingTodos.Clear();
        coroutine = null;
        canvasGroup.alpha = 0f;
        if (uiManager != null) uiManager.Pop(this);
        Closed?.Invoke(this);
        onHide?.Invoke();
    }

    private void OnDisable()
    {
        if (coroutine != null) StopCoroutine(coroutine);
        FinishClose();
    }

    private void OnDestroy()
    {
        FinishClose();
        foreach (var pair in todoCallbacks)
            if (pair.Key != null) pair.Key.OnAnimationFinished -= pair.Value;
        foreach (var item in registeredItems)
        {
            if (item.condition != null)
                item.condition.OnCheck -= item.callback;
        }
    }

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
    private bool displayRequested;
    private enum DisplayState { Hidden, Entering, Checking, Waiting, Background, Closing }
    private DisplayState displayState;
    private UIManager uiManager;
    private bool initialized;
    private bool layoutFinished;
    private bool fadeFinished;
    private int readyFrame;
    private readonly HashSet<Todo> pendingTodos = new HashSet<Todo>();
    private readonly Dictionary<Todo, Action> todoCallbacks = new Dictionary<Todo, Action>();
    private readonly List<(ClearCondition condition, Action callback)>
        registeredItems = new List<(ClearCondition condition, Action callback)>();
    public bool IsDisplayed => displayRequested;
    public bool IsClosing => displayState == DisplayState.Closing;
    public bool CanStartClearCheck
    {
        get
        {
            if (displayState == DisplayState.Entering)
                return false;

            if (displayState == DisplayState.Checking)
                return false;

            if (displayState == DisplayState.Closing)
                return false;

            return true;
        }
    }

    public int sortingOrder { get; set; }
    public bool inputActionEnabled { get; set; }
    public event Action onShow;
    public event Action onHide;
    public event Action<Card> Closing;
    public event Action<Card> Closed;

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
        if (!displayRequested)
        {
            canvasGroup.alpha = 0f;
            foreach (Todo todo in todoList)
            {
                if (todo != null)
                    todo.Close();
            }
        }

    }
    #region 초기화 기능 및 초기화에 넣을 기능
    public void InitCard()
    {
        if (initialized)
            return;
        initialized = true;
        completedTodoCount = 0;
        if (todoList == null) todoList = new List<Todo>();
        todoList.Clear();
        if (itemInTodoes == null) return;
        foreach (var item in itemInTodoes)
        {
            if (item.todo == null) continue;
            if (!todoCallbacks.ContainsKey(item.todo))
            {
                Todo todo = item.todo;
                Action callback = () => OnTodoFinished(todo);
                todoCallbacks.Add(todo, callback);
                todo.SubscribeTodoAction(callback);
                todoList.Add(todo);
            }
            if (item.item != null)
            {
                Action callback = () =>
                {
                    if (item.item == null || item.todo == null)
                        return;

                    if (!item.item.IsSatisfied())
                        return;

                    item.todo.DoSuccess();
                };

                item.item.AddOnCheck(callback);
                registeredItems.Add((item.item, callback));
            }
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
        if (displayState != DisplayState.Checking) return;
        completedTodoCount++;
        if (completedTodoCount >= todoList.Count)
        {
            BeginWaitForInput();
            OnTodoAnimationFinished?.Invoke(this);
        }
    }
    #endregion

    public void PlayAnimation(string animationName)
    {
        animator.Play(animationName);
    }

    public void ShowForLayout()
    {
        if (!gameObject.activeSelf) gameObject.SetActive(true);
        if (!isActiveAndEnabled) return;
        InitCard();
        displayRequested = true;
        displayState = DisplayState.Entering;
        layoutFinished = false;
        fadeFinished = false;
        pendingTodos.Clear();
        if (coroutine != null) StopCoroutine(coroutine);
        coroutine = null;
        foreach (Todo todo in todoList)
            if (todo != null) todo.Close();
        PlayAnimation("Default");
        canvasGroup.alpha = 1f;
        rectTransform.SetAsLastSibling();
        uiManager = UIManager.instance;
        bool wasOpen = uiManager.showns.Contains(this);
        uiManager.Push(this);
        if (!wasOpen) onShow?.Invoke();
    }

    public void ShowAsBackground()
    {
        if (!gameObject.activeSelf) gameObject.SetActive(true);
        if (!isActiveAndEnabled) return;
        InitCard();
        if (coroutine != null) StopCoroutine(coroutine);
        coroutine = null;
        pendingTodos.Clear();
        displayRequested = true;
        displayState = DisplayState.Background;
        inputActionEnabled = false;
        canvasGroup.alpha = 1f;
        if (uiManager != null) uiManager.Pop(this);
        SetVisible(false);
        rectTransform.SetAsLastSibling();
    }

    public void HideAsBackground(float duration)
    {
        if (!displayRequested) return;
        if (coroutine != null) StopCoroutine(coroutine);
        coroutine = null;
        displayState = DisplayState.Closing;
        pendingTodos.Clear();
        inputActionEnabled = false;
        SetVisible(false);
        if (duration <= 0f)
            FinishClose();
        else
            coroutine = StartCoroutine(PlayCloseAnimation(this, duration));
    }

    public void RemoveCard(Card card = null)
    {
        if (card == null) card = this;
        if (!card.displayRequested || card.IsClosing) return;
        card.displayState = DisplayState.Closing;
        card.pendingTodos.Clear();
        if (card.coroutine != null) card.StopCoroutine(card.coroutine);
        card.PlayAnimation("Default");
        card.Closing?.Invoke(card);
        card.coroutine = card.StartCoroutine(card.PlayCloseAnimation(card, card.animDuration));
    }
    public void HideCard()
    {
        Hide();
    }

    //클리어 가 실행되면 실행될 애니메이션 중간에 타이밍을 조절해야함
    public void SuccessClearStage()
    {
        if (!displayRequested || displayState != DisplayState.Entering) return;
        InitCard();
        if (coroutine != null) StopCoroutine(coroutine);
        coroutine = null;
        completedTodoCount = 0;
        displayState = DisplayState.Checking;
        pendingTodos.Clear();
        foreach (Todo todo in todoList)
            if (todo != null) pendingTodos.Add(todo);
        if (pendingTodos.Count == 0)
        {
            BeginWaitForInput();
            OnTodoAnimationFinished?.Invoke(this);
            return;
        }
        coroutine = StartCoroutine(CheckCondition());
    }
    //Card 진입이 진행되면 실행될 메서드
    public void NextStep()
    {
        BeginWaitForInput();
    }
    public void StartGameCardOpen()
    {
        ShowForLayout();
        PlayAnimation("In_Start_ForSys");
    }
    public void CheckClearCard()
    {
        CardManager.Instance.CheckClearCard();
    }
    #region 코루틴
    IEnumerator CheckCondition()
    {
        foreach (Todo todo in todoList)
        {
            if (displayState != DisplayState.Checking) yield break;
            if (todo != null) todo.CheckClear();
            yield return new WaitForSecondsRealtime(todoDelay);
        }
    }
    protected IEnumerator PlayCloseAnimation(Card card, float duration)
    {
        float startAlpha = card.CanvasGroup.alpha;
        yield return null;
        yield return UIAnimationRoutine.Run(
            duration,
            progress => progress * progress,
            t => card.CanvasGroup.alpha = Mathf.Lerp(startAlpha, 0f, t));

        card.CanvasGroup.alpha = 0f;
        card.FinishClose();
    }

    #endregion
}
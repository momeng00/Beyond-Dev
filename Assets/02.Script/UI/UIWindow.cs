using System;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Canvas))]
public class UIWindow : UIBase, IUI
{
    //private Canvas _canvas;
    [Header("옵션: 내부 버튼 이동 기능")]
    public UIKeyNavigator keyNavigator; // 인스펙터에서 할당하거나 Awake에서 찾음
    public UISequenceController uISequenceController;
    public event Action onShow;
    public event Action onHide;
    public UnityEvent onShowed;
    public UnityEvent onHided;
    public int test;
    // --- IUI 인터페이스 구현 (매니저와의 약속) ---
    public int sortingOrder
    {
        get => 0;
        set => test=value;
        //get => _canvas.sortingOrder;
        //set => _canvas.sortingOrder = value;
    }
    public bool inputActionEnabled { get { return ActionEnabled; } set { ActionEnabled = value; } }
    private bool ActionEnabled = false;
    // ----------------------------------------

    protected override void Awake()
    {
        base.Awake(); // 부모(UIBase)의 Awake 실행 (초기 위치 저장 등)
        //_canvas = GetComponent<Canvas>();
      
    }

    protected override void Start()
    {
        base.Start();
        InitUI();

        // 팝업창은 태어나자마자 매니저에 등록해야 함
        if (UIManager.instance != null)
        {
            // Register 로직은 UIManager가 자동으로 처리하지 않는다면 
            // 보통 Show() 할 때 Push 하므로 여기선 생략 가능하거나
            // 관리용 리스트에만 넣을 수 있습니다.
            // 여기서는 UIManager 구조상 Push 때 등록되므로 비워둡니다.
            
        }
        // 시작 시 자동으로 꺼두고 싶다면 활성화 (선택)
        
    }

    // UIBase의 Open 기능을 확장(Override)해서 매니저 호출 추가
    public override void Open()
    {
        // 1. 매니저에게 "나 열렸다" 보고 (순서 정리)
        UIManager.instance.Push(this);
        // 2. 부모의 Open 실행 (애니메이션 재생)
        base.Open();
        onShow?.Invoke();
        onShowed?.Invoke();
        if (uISequenceController != null)
        {
            uISequenceController.Play();
        }
       
    }

    // UIBase의 Close 기능을 확장
    public override void Close()
    {
        // 1. 부모의 Close 실행 (애니메이션 재생)
        base.Close();
        // 2. 매니저에게 "나 닫혔다" 보고
        UIManager.instance.Pop(this);
        onHide?.Invoke();
        onHided?.Invoke();
    }
    public void InitUI()
    {
        base.Close();
    }
    // IUI 구현: 매니저 인터페이스 맞추기용 (이름 통일)
    public void Show() => Open();
    public void Hide() => Close();

    // 입력 감지 (ESC 등)
    public void InputAction()
    {
        if (keyNavigator != null)
        {
            // 상하좌우 키 로직 분배
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                keyNavigator.NextElement();
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                keyNavigator.PreElement();
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                keyNavigator.NextCarouselElement();
            }
            else if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                keyNavigator.PreCarouselElement();
            }
            else if (Input.GetKeyDown(KeyCode.Return))
            {
                keyNavigator.SelectElement();
            }
        }
    }
    public virtual void SetVisible(bool state)
    {
        if (canvasGroup == null)
            return;
        //canvasGroup.alpha = state ? 1 : 0;
        canvasGroup.interactable = state;
        canvasGroup.blocksRaycasts = state;
    }
    private void Update()
    {
        // 매니저가 허락했을 때만 입력 감지
        if (inputActionEnabled)
        {
            InputAction();
        }
    }
}
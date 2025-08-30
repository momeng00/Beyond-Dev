using CarouselUI;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public enum MenuName
{
    Menu_Main,
    Menu_Setting,
}
public class UIManager : MonoBehaviour
{
    //치트
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            ChangeGroup(MenuName.Menu_Main.ToString());
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            ChangeGroup(MenuName.Menu_Setting.ToString());
        }
    }
    private Animator _animator;
    private MenuName _menuName;
    public MenuName menuName
    {
        get { return _menuName; }
        set 
        {
            _menuName = value;
            if(IsValidGroup(value.ToString()))
            {
                _animator.Play(menuName.ToString());
            }
        }
    }
    public List<UIElementGroup> uiElementGroups;
    private Dictionary<string, List<UIElement>> uiElements;
    private List<UIElement> uiElement;
    [SerializeField]private UIElement currentElement; //호버 대상 임시확인
    private void Awake()
    {
        _animator = gameObject.GetComponent<Animator>();
    }
    private void Start()
    {
        Initialize();
        uiElements = new Dictionary<string, List<UIElement>>();
        foreach (var group in uiElementGroups)
        {
            if (!uiElements.ContainsKey(group.key.ToString()))
            {
                uiElements.Add(group.key.ToString(), group.uIElements);
            }
        }
        uiElement = uiElements[menuName.ToString()];
        currentElement = uiElement[0];
        _animator.Play(menuName.ToString());
    }

    public void Initialize()
    {
        menuName = MenuName.Menu_Main;
        InputSystem.Instance.RegisterAction(KeyState.Play_Key,KeyCode.RightArrow, NextElement);
        InputSystem.Instance.RegisterAction(KeyState.Play_Key, KeyCode.DownArrow, NextCarouselElement);
        InputSystem.Instance.RegisterAction(KeyState.Play_Key, KeyCode.LeftArrow, PreElement);
        InputSystem.Instance.RegisterAction(KeyState.Play_Key, KeyCode.UpArrow, PreCarouselElement);
        InputSystem.Instance.RegisterAction(KeyState.Play_Key, KeyCode.Return, SelectElement);
        InputSystem.Instance.RegisterAction(KeyState.Play_Key, KeyCode.Escape, () => ChangeGroup(MenuName.Menu_Main.ToString()) );
    }

    //Unity Event에 넣으려면 string이 들어가야하고 이걸 enum으로 변경하려면...
    public void ChangeGroup(string groupName)
    {
        if (IsValidGroup(groupName))
        {
            CanvasGroup oldGroupCanvas = currentElement.gameObject.GetComponentInParent<CanvasGroup>();
            if (uiElements.ContainsKey(groupName))
            {
                oldGroupCanvas.alpha = 0f;
                oldGroupCanvas.interactable = false;
                oldGroupCanvas.blocksRaycasts = false;
                uiElement = uiElements[groupName];

                currentElement = uiElement[0]; // 새 그룹의 첫 번째 요소를 기본 선택
                CanvasGroup current = currentElement.gameObject.GetComponentInParent<CanvasGroup>();
                current.interactable = true;
                current.blocksRaycasts = true;
                if (Enum.TryParse(groupName, out MenuName menuName))
                {
                    this.menuName = menuName;
                }
                
                UpdateUIElement(); // UI 업데이트
                Debug.Log($"Changed to group: {groupName}");
            }
            else
            {
                Debug.LogError($"UI Group with key '{groupName}' not found!");
            }
        }
    }

    public bool IsValidGroup(string groupName)
    {
        // Enum.IsDefined(typeof(확인할 Enum 타입), 확인할 문자열)
        return Enum.IsDefined(typeof(MenuName), groupName);
    }

   

    public void NextElement()
    {
        //CarouselUIElement 사용
        CarouselUIElement carouselElement = currentElement as CarouselUIElement;
        if(carouselElement != null)
        {
            carouselElement.PressNext();
            UpdateUIElement();
            return;
        }
        UIElement nextElement;
        //아닐경우
        if ( (uiElement.IndexOf(currentElement) + 1) >= uiElement.Count)
        {
            nextElement = uiElement[0];
        }
        else
        {
            nextElement = uiElement[uiElement.IndexOf(currentElement) + 1];
        }
        currentElement.UnSelected();
        currentElement = nextElement;
        currentElement.Selected();
    }
    public void PreElement()
    {
        //CarouselUIElement 사용
        CarouselUIElement carouselElement = currentElement as CarouselUIElement;
        if (carouselElement != null)
        {
            carouselElement.PressPrevious();
            UpdateUIElement();
            return;
        }
        UIElement nextElement;
        //아닐경우
        if (uiElement.IndexOf(currentElement) <= 0)
        {
            nextElement = uiElement[uiElement.Count - 1];
        }
        else
        {
            nextElement = uiElement[uiElement.IndexOf(currentElement) - 1];
        }
        currentElement.UnSelected();
        currentElement = nextElement;
        currentElement.Selected();

    }
    public void NextCarouselElement()
    {
        UIElement nextElement;
        if ((uiElement.IndexOf(currentElement) + 1) >= uiElement.Count)
        {
            nextElement = uiElement[0];
        }
        else
        {
            nextElement = uiElement[uiElement.IndexOf(currentElement) + 1];
        }
        currentElement.UnSelected();
        currentElement = nextElement;
        currentElement.Selected();
    }
    public void PreCarouselElement()
    {
        UIElement nextElement;
        if (uiElement.IndexOf(currentElement) <= 0)
        {
            nextElement = uiElement[uiElement.Count - 1];
        }
        else
        {
            nextElement = uiElement[uiElement.IndexOf(currentElement) - 1];
        }
        currentElement.UnSelected();
        currentElement = nextElement;
        currentElement.Selected();
    }
    public void SelectElement()
    {
        if (currentElement == null)
        {
            Debug.Log("currentElement가 없습니다.");
            return;
        }
        currentElement.OnCustomClick?.Invoke();
        UpdateUIElement();
    }
    public void CheckCurrentElement()
    {
        currentElement.Selected();
    }
    public void UpdateUIElement()
    {
        //currentElement를 호버되는 방식으로 업데이트 되어야함.
    }
    
}
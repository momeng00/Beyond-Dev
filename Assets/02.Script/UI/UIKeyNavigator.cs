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
public class UIKeyNavigator : MonoBehaviour
{
    //치트
    private void Update()
    {
        
    }

   
    
    public List<UIElement> uiElements;
    [SerializeField]private UIElement currentElement;
    private void Awake()
    {

    }
    //public List<UIElementGroup> uiElementGroups;
    //private Dictionary<string, List<UIElement>> uiElements; 그룹 자체를 UI가 관리하게 되므로 없어져도 될것같음
    private void Start()
    {
        Initialize();
        //uiElements = new Dictionary<string, List<UIElement>>();
        //foreach (var group in uiElementGroups)
        //{
        //    if (!uiElements.ContainsKey(group.key.ToString()))
        //    {
        //        uiElements.Add(group.key.ToString(), group.uIElements);
        //    }
        //}
        //uiElement = uiElements[menuName.ToString()];
    }
    //Unity Event에 넣으려면 string이 들어가야하고 이걸 enum으로 변경하려면...
    //public void ChangeGroup(string groupName)
    //{
    //    if (IsValidGroup(groupName))
    //    {
    //        CanvasGroup oldGroupCanvas = currentElement.gameObject.GetComponentInParent<CanvasGroup>();
    //        if (uiElements.ContainsKey(groupName))
    //        {
    //            oldGroupCanvas.alpha = 0f;
    //            oldGroupCanvas.interactable = false;
    //            oldGroupCanvas.blocksRaycasts = false;
    //            uiElement = uiElements[groupName];

    //            currentElement = uiElement[0]; // 새 그룹의 첫 번째 요소를 기본 선택
    //            CanvasGroup current = currentElement.gameObject.GetComponentInParent<CanvasGroup>();
    //            current.interactable = true;
    //            current.blocksRaycasts = true;
    //            if (Enum.TryParse(groupName, out MenuName menuName))
    //            {
    //                this.menuName = menuName;
    //            }

    //            UpdateUIElement(); // UI 업데이트
    //            Debug.Log($"Changed to group: {groupName}");
    //        }
    //        else
    //        {
    //            Debug.LogError($"UI Group with key '{groupName}' not found!");
    //        }
    //    }
    //}
    public void Initialize()
    {
        if (uiElements.Count > 0)
        {
            // 기존 선택 해제
            if (currentElement != null) currentElement.UnSelected();
            
            // 첫 번째 요소 선택
            currentElement = uiElements[0];
            currentElement.Selected();
        }

        //InputSystem.Instance.RegisterAction(KeyState.Play_Key,KeyCode.RightArrow, NextElement);
        //InputSystem.Instance.RegisterAction(KeyState.Play_Key, KeyCode.DownArrow, NextCarouselElement);
        //InputSystem.Instance.RegisterAction(KeyState.Play_Key, KeyCode.LeftArrow, PreElement);
        //InputSystem.Instance.RegisterAction(KeyState.Play_Key, KeyCode.UpArrow, PreCarouselElement);
        //InputSystem.Instance.RegisterAction(KeyState.Play_Key, KeyCode.Return, SelectElement);
        //InputSystem.Instance.RegisterAction(KeyState.Play_Key, KeyCode.Escape, () => ChangeGroup(MenuName.Menu_Main.ToString()) );
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
        if ( (uiElements.IndexOf(currentElement) + 1) >= uiElements.Count)
        {
            nextElement = uiElements[0];
        }
        else
        {
            nextElement = uiElements[uiElements.IndexOf(currentElement) + 1];
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
        if (uiElements.IndexOf(currentElement) <= 0)
        {
            nextElement = uiElements[uiElements.Count - 1];
        }
        else
        {
            nextElement = uiElements[uiElements.IndexOf(currentElement) - 1];
        }
        currentElement.UnSelected();
        currentElement = nextElement;
        currentElement.Selected();

    }
    public void NextCarouselElement()
    {
        UIElement nextElement;
        if ((uiElements.IndexOf(currentElement) + 1) >= uiElements.Count)
        {
            nextElement = uiElements[0];
        }
        else
        {
            nextElement = uiElements[uiElements.IndexOf(currentElement) + 1];
        }
        currentElement.UnSelected();
        currentElement = nextElement;
        currentElement.Selected();
    }
    public void PreCarouselElement()
    {
        UIElement nextElement;
        if (uiElements.IndexOf(currentElement) <= 0)
        {
            nextElement = uiElements[uiElements.Count - 1];
        }
        else
        {
            nextElement = uiElements[uiElements.IndexOf(currentElement) - 1];
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

    public void UpdateUIElement()
    {
        //currentElement를 호버되는 방식으로 업데이트 되어야함.
    }
    
}
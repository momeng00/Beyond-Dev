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
    public List<UIElement> uiElements;
    [SerializeField]private UIElement currentElement;

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
        if (uiElements == null || uiElements.Count == 0)
            return;

        foreach (UIElement element in uiElements)
        {
            if (element == null)
                continue;

            if (currentElement != null)
                currentElement.UnSelected();

            currentElement = element;
            currentElement.Selected();
            return;
        }

        //InputSystem.Instance.RegisterAction(KeyState.Play_Key, KeyCode.RightArrow, NextElement);
        //InputSystem.Instance.RegisterAction(KeyState.Play_Key, KeyCode.DownArrow, NextCarouselElement);
        //InputSystem.Instance.RegisterAction(KeyState.Play_Key, KeyCode.LeftArrow, PreElement);
        //InputSystem.Instance.RegisterAction(KeyState.Play_Key, KeyCode.UpArrow, PreCarouselElement);
        //InputSystem.Instance.RegisterAction(KeyState.Play_Key, KeyCode.Return, SelectElement);
        //InputSystem.Instance.RegisterAction(KeyState.Play_Key, KeyCode.Escape, () => ChangeGroup(MenuName.Menu_Main.ToString()));
    }

    

    public bool IsValidGroup(string groupName)
    {
        // Enum.IsDefined(typeof(확인할 Enum 타입), 확인할 문자열)
        return Enum.IsDefined(typeof(MenuName), groupName);
    }

   

    public void NextElement()
    {
        if (currentElement is CarouselUIElement carouselElement
            && carouselElement != null)
        {
            carouselElement.PressNext();
            UpdateUIElement();
            return;
        }

        MoveSelection(1);
    }

    public void PreElement()
    {
        if (currentElement is CarouselUIElement carouselElement
            && carouselElement != null)
        {
            carouselElement.PressPrevious();
            UpdateUIElement();
            return;
        }

        MoveSelection(-1);
    }

    public void NextCarouselElement()
    {
        MoveSelection(1);
    }

    public void PreCarouselElement()
    {
        MoveSelection(-1);
    }

    private void MoveSelection(int direction)
    {
        if (uiElements == null || uiElements.Count == 0)
            return;

        int count = uiElements.Count;
        int currentIndex = currentElement != null
            ? uiElements.IndexOf(currentElement)
            : -1;

        if (currentIndex < 0)
            currentIndex = direction > 0 ? -1 : count;

        int nextIndex = currentIndex;

        for (int i = 0; i < count; i++)
        {
            nextIndex = (nextIndex + direction + count) % count;

            UIElement nextElement = uiElements[nextIndex];
            if (nextElement == null)
                continue;

            if (currentElement != null)
                currentElement.UnSelected();

            currentElement = nextElement;
            currentElement.Selected();
            return;
        }
    }

    public void SelectElement()
    {
        if (currentElement == null)
        {
            Debug.Log("currentElement가 없습니다.");
            return;
        }
        currentElement.Action();
        UpdateUIElement();
    }

    public void UpdateUIElement()
    {
        //currentElement를 호버되는 방식으로 업데이트 되어야함.
    }
    
}
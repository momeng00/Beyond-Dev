using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    private static UIManager _instance;
    public static UIManager instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new GameObject("UIManagement").AddComponent<UIManager>();
            }
            return _instance;
        }
    }
    public Dictionary<Type, IUI> uis = new Dictionary<Type, IUI>(); // 등록된 모든 UI
    public LinkedList<IUI> showns = new LinkedList<IUI>(); // 현재 보여지고있는 팝업 UI 들

    public T Get<T>()
        where T : IUI
    {
        if (uis.TryGetValue(typeof(T), out IUI ui))
            return (T)ui;
        else
            throw new Exception($"[UIManager] : {typeof(T)} has not been registered but you tried to get it ..");
    }
    public void Register(IUI ui)
    {
        Type type = ui.GetType();
        if (uis.TryAdd(type, ui) == false)
            throw new Exception($"[UIManager] : {type} already registered but you tried to add it again..!");
        Debug.Log($"[UIManager] : Registered {type}.");
    }
    public void Push(IUI ui)
    {
        // 이미 젤 뒤에있으면 안함
        if (showns.Count > 0 && showns.Last.Value == ui)
            return;

        // 가장뒤에있던 UI 보다 뒤로 보내기 
        int sortingOrder = 0;
        if (showns.Last?.Value != null)
        {
            sortingOrder = showns.Last.Value.sortingOrder + 1;
            showns.Last.Value.inputActionEnabled = false;
        }
        ui.sortingOrder = sortingOrder;
        ui.inputActionEnabled = true;
        showns.Remove(ui);
        showns.AddLast(ui);

        //커서를 보이게 혹은 안보이게 하는 방법임 삭제 가능.
        //if (showns.Count == 1)
        //{
        //    Cursor.visible = true;
        //    Cursor.lockState = CursorLockMode.Confined;
        //}

    }

    public void Pop(IUI ui)
    {
        // 빼려는게 하필 마지막꺼면서 마지막꺼 앞에 하나이상 있으면 InputAction 활성화 바톤터치
        if (showns.Count > 1 &&
            showns.Last.Value == ui)
        {
            showns.Last.Previous.Value.inputActionEnabled = true;
            // [변경] 즉시 켜지 않고 코루틴으로 한 프레임 뒤에 켬
            StartCoroutine(EnableNextUI(showns.Last.Previous.Value));
        }


        showns.Remove(ui); // ui 뺌
        //마찬가지로 커서를 안보이게 하는 방법임. 삭제 가능
        //if (showns.Count == 0)
        //{
        //    Cursor.visible = false;
        //    Cursor.lockState = CursorLockMode.Locked;
        //}
    }
    IEnumerator EnableNextUI(IUI nextUI)
    {
        // 한 프레임 대기 (이 사이에 GetKeyDown 상태가 꺼짐)
        yield return null;

        if (nextUI != null)
            nextUI.inputActionEnabled = true;
    }
    public void HideLast()
    {
        if (showns.Count <= 0)
            return;

        showns.Last.Value.Hide();
    }
}
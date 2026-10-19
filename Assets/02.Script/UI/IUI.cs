using System;
using UnityEngine;

public interface IUI
{
    int sortingOrder { get; set; }
    bool inputActionEnabled { get; set; }

    void Show();
    void Hide();
    void SetVisible(bool state);
    event Action onShow;
    event Action onHide;
}

public interface IUIInputHandler
{
    void HandleInput(KeyCode key);
}

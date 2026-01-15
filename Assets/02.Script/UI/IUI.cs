using System;

public interface IUI
{
    int sortingOrder { get; set; }
    bool inputActionEnabled { get; set; }

    void InputAction();
    void Show();
    void Hide();
    void SetVisible(bool state);
    event Action onShow;
    event Action onHide;
}
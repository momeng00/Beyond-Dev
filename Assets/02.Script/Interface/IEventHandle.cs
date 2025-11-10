using System;

public interface IEventHandle
{
    //이벤트를 실행시킬 주최자
    public event Action<bool> OnToggleEvent;
    public void ToggleEventAddListener(Action<bool> action);
    public void ToggleEventDeleteListener(Action<bool> action);

}
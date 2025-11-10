
public interface IEventListener
{
    //이벤트를 등록할 주체
    public void ToggleEvent(bool state);
    public int ToggleEventPriority { get; }
}
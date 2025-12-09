public interface ISwitchable
{
    Switch Switch { get; }
    bool SwitchOn(bool value);

}
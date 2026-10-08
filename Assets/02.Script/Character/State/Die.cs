public class Die : CharacterStateBase
{
    public override void EnterState()
    {
        base.EnterState();
        AudioManager.Instance.PlaySFXAudio(AudioName.Die);
    }
    public override void ExitState()
    {
        base.ExitState();
    }
    public override CharacterStateID OnUpdateState()
    {
        return CharacterStateID.Die;
    }
}
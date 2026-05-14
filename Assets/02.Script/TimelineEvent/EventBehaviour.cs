using UnityEngine.Events;
using UnityEngine.Playables;

public class EventBehaviour : PlayableBehaviour
{
    public UnityEvent onPlay;  // TimelineEvent 대신 UnityEvent 직접 사용

    bool played;

    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        if (played) return;
        played = true;
        onPlay?.Invoke();
    }

    public override void OnBehaviourPause(Playable playable, FrameData info)
    {
        played = false;
    }
}
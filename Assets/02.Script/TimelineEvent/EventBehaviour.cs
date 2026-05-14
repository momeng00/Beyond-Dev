using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;

public class EventBehaviour : PlayableBehaviour
{
    public TimelineEventBridge target;

    private bool _played;

    public override void OnBehaviourPlay(Playable playable, FrameData info)
    {
        _played = false;
    }

    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        if (_played || target == null) return;
        _played = true;
        target.Play();
    }

    public override void OnBehaviourPause(Playable playable, FrameData info)
    {
        _played = false;
    }
    //public GameObject targetObject;

    //public UnityEvent onPlay;

    //bool played;

    //public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    //{
    //    if (played) return;

    //    played = true;

    //    Debug.Log(targetObject);

    //    onPlay?.Invoke();
    //}

    //public override void OnBehaviourPause(Playable playable, FrameData info)
    //{
    //    played = false;
    //}
}
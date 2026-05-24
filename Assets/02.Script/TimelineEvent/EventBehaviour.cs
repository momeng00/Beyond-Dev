using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;

public class EventBehaviour : PlayableBehaviour
{
    public TimelineEventBridge target;
    public UnityEvent onPlay;
    private bool _played;

    public override void OnBehaviourPlay(Playable playable, FrameData info)
    {
        _played = false;
    }

    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        if (_played || target == null) return;
        _played = true;
        // target이 있으면 Bridge 실행, 없으면 onPlay 실행
        if (target != null)
            target.Play();
        else
            onPlay?.Invoke();
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
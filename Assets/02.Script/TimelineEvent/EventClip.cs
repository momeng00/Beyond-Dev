using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class EventClip : PlayableAsset, ITimelineClipAsset
{
    public ExposedReference<TimelineEventBridge> target; // 씬 오브젝트 참조
    public UnityEvent onPlay;
    public ClipCaps clipCaps => ClipCaps.None;

    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<EventBehaviour>.Create(graph);
        var behaviour = playable.GetBehaviour();
        behaviour.target = target.Resolve(graph.GetResolver());
        behaviour.onPlay = onPlay;
        return playable;
    }


    //public ExposedReference<GameObject> targetObject;

    //public UnityEvent onPlay;

    //public ClipCaps clipCaps => ClipCaps.None;

    //public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    //{
    //    var playable = ScriptPlayable<EventBehaviour>.Create(graph);

    //    var behaviour = playable.GetBehaviour();

    //    behaviour.targetObject =
    //        targetObject.Resolve(graph.GetResolver());

    //    behaviour.onPlay = onPlay;

    //    return playable;
    //}
}
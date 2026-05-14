using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class EventClip : PlayableAsset, ITimelineClipAsset
{
    public UnityEvent onPlay;  // 동일하게 변경

    public ClipCaps clipCaps => ClipCaps.None;

    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<EventBehaviour>.Create(graph);
        var behaviour = playable.GetBehaviour();
        behaviour.onPlay = onPlay;
        return playable;
    }
}
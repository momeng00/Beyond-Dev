using UnityEngine;
using UnityEngine.Events;

public class TimelineEventBridge : MonoBehaviour
{
    public UnityEvent onPlay;

    public void Play() => onPlay?.Invoke();
}
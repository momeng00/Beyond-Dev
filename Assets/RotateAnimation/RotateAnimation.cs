using UnityEngine;

[DisallowMultipleComponent]
[AddComponentMenu("Animation/Rotate Animation")]
public sealed class RotateAnimation : MonoBehaviour
{
    public enum RepeatMode { Once, Loop, PingPong }
    [SerializeField] bool playOnAwake = true;
    [SerializeField] Vector3 axis = Vector3.up;
    [SerializeField] float angle = 360f;
    [SerializeField] Space space = Space.Self;
    [SerializeField, Min(0.001f)] float duration = 2f;
    [SerializeField] RepeatMode repeat = RepeatMode.Loop;
    [SerializeField] AnimationCurve easing = AnimationCurve.Linear(0, 0, 1, 1);
    [SerializeField] bool useUnscaledTime;

    Quaternion initialLocal, initialWorld;
    bool initialized;
    public bool IsPlaying { get; private set; }
    public float ElapsedTime { get; private set; }
    public float Duration => Mathf.Max(0.001f, duration);
    public RepeatMode Repeat => repeat;

    void Start() { if (playOnAwake) Play(); }
    void Update()
    {
        if (!IsPlaying) return;
        SetElapsedTime(ElapsedTime + (useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime));
        if (repeat == RepeatMode.Once && ElapsedTime >= Duration) Pause();
    }
    void OnDisable() => Pause();

    void CaptureInitialRotation()
    {
        if (initialized) return;
        initialLocal = transform.localRotation;
        initialWorld = transform.rotation;
        initialized = true;
    }

    public void Play()
    {
        CaptureInitialRotation();
        if (repeat == RepeatMode.Once && ElapsedTime >= Duration) SetElapsedTime(0);
        IsPlaying = true;
    }
    public void Pause() => IsPlaying = false;
    public void Restart() { Stop(); Play(); }
    public void Stop()
    {
        Pause();
        ElapsedTime = 0;
        if (initialized) transform.localRotation = initialLocal;
        initialized = false;
    }
    public void SetElapsedTime(float seconds)
    {
        CaptureInitialRotation();
        ElapsedTime = Mathf.Max(0, seconds);
        if (repeat == RepeatMode.Once) ElapsedTime = Mathf.Min(ElapsedTime, Duration);
        transform.localRotation = EvaluateLocalRotation(initialLocal, initialWorld, ElapsedTime);
    }

    public float ProgressAt(float seconds)
    {
        float t = Mathf.Max(0, seconds) / Duration;
        switch (repeat)
        {
            case RepeatMode.Loop: return Mathf.Repeat(t, 1f);
            case RepeatMode.PingPong: return Mathf.PingPong(t, 1f);
            default: return Mathf.Clamp01(t);
        }
    }

    // AngleAxis preserves full turns (360, 720, etc.), unlike endpoint Slerp.
    public Quaternion EvaluateLocalRotation(Quaternion startLocal, Quaternion startWorld, float seconds)
    {
        float t = ProgressAt(seconds);
        float amount = easing == null ? t : easing.Evaluate(t);
        Quaternion delta = axis.sqrMagnitude < 0.000001f
            ? Quaternion.identity : Quaternion.AngleAxis(angle * amount, axis.normalized);
        if (space == Space.Self) return startLocal * delta;
        Quaternion world = delta * startWorld;
        return transform.parent == null ? world : Quaternion.Inverse(transform.parent.rotation) * world;
    }
}

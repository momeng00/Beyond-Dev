using System.Collections;
using UnityEngine;
using UnityEngine.Video;

public class DualVideoOffsetPlayer : MonoBehaviour
{
    [Header("Video Players")]
    [SerializeField] private VideoPlayer videoPlayerA;
    [SerializeField] private VideoPlayer videoPlayerB;

    [Header("Start Offset (Seconds)")]
    [Min(0f)]
    [SerializeField] private double offsetA = 0.0;

    [Min(0f)]
    [SerializeField] private double offsetB = 0.5;

    [Header("Options")]
    [SerializeField] private bool playOnStart = true;
    [SerializeField] private bool loop = true;

    private Coroutine playCoroutine;

    private void Start()
    {
        if (playOnStart)
        {
            PlayVideos();
        }
    }

    public void PlayVideos()
    {
        if (playCoroutine != null)
        {
            StopCoroutine(playCoroutine);
        }

        playCoroutine = StartCoroutine(PrepareAndPlay());
    }

    public void StopVideos()
    {
        if (playCoroutine != null)
        {
            StopCoroutine(playCoroutine);
            playCoroutine = null;
        }

        if (videoPlayerA != null)
        {
            videoPlayerA.Stop();
        }

        if (videoPlayerB != null)
        {
            videoPlayerB.Stop();
        }
    }

    private IEnumerator PrepareAndPlay()
    {
        if (videoPlayerA == null || videoPlayerB == null)
        {
            Debug.LogError("VideoPlayer A 또는 B가 연결되지 않았습니다.", this);
            yield break;
        }

        videoPlayerA.playOnAwake = false;
        videoPlayerB.playOnAwake = false;

        videoPlayerA.isLooping = loop;
        videoPlayerB.isLooping = loop;

        videoPlayerA.Stop();
        videoPlayerB.Stop();

        videoPlayerA.Prepare();
        videoPlayerB.Prepare();

        yield return new WaitUntil(() =>
            videoPlayerA.isPrepared &&
            videoPlayerB.isPrepared
        );

        videoPlayerA.time = ClampOffset(offsetA, videoPlayerA.length);
        videoPlayerB.time = ClampOffset(offsetB, videoPlayerB.length);

        videoPlayerA.Play();
        videoPlayerB.Play();

        playCoroutine = null;
    }

    private double ClampOffset(double offset, double videoLength)
    {
        if (videoLength <= 0)
        {
            return 0;
        }

        return offset % videoLength;
    }
}
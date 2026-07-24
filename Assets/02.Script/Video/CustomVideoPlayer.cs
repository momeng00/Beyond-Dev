using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.Video;

[RequireComponent(typeof(VideoPlayer))]
public class CustomVideoPlayer : MonoBehaviour
{
    private VideoPlayer _vidPlayer;
    public RenderTexture _vidTexture;
    private RawImage _vidImage;
    public VideoData videoName;
    public UnityEvent onStarted;
    public UnityEvent onFinished;
    //videoPlayer.loopPointReached //영상이 끝지점에 다가가면 자동으로 호출하는 이벤트 (유니티 기본기능)
    //videoPlayer.started //마찬가지로 시작할때
    //videoPlayer.prepareCompleted    // Prepare 완료
    //videoPlayer.started             // Play가 시작됨
    //videoPlayer.loopPointReached    // 영상 끝남
    //videoPlayer.seekCompleted       // 특정 시간으로 이동 완료
    //videoPlayer.errorReceived       // 오류 발생
    //videoPlayer.frameDropped        // 프레임 드랍
    private void Awake()
    {
        _vidImage = GetComponent<RawImage>();
        _vidPlayer = GetComponent<VideoPlayer>();
    }
    private void Start()
    {
        PrepareVideo(videoName);
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            PrepareVideo(VideoData.Sample_Yong);
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            PlayVideo(_vidPlayer);
        }
    }
    public void PrepareVideo(VideoData videoName)
    {
        VideoClip videoClip = VideoManager.Instance.Prepare(videoName);

        _vidPlayer.targetTexture = _vidTexture;
        _vidImage.texture = _vidTexture;
        _vidPlayer.clip = videoClip;

        _vidPlayer.Prepare();
        _vidPlayer.loopPointReached += EndVideo;
    }
    public void PlayVideo(VideoPlayer video)
    {
        _vidPlayer.Play();
    }
    public void Pause(VideoPlayer video)
    {

    }

    public void EndVideo(VideoPlayer video)
    {
        onFinished?.Invoke();
        Destroy(this.gameObject);
    }
}
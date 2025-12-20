using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class TimeLineController : MonoBehaviour
{
    public PlayableDirector director;
    // 현재 입력을 기다리는 중인가?
    private bool isWaitingForInput = false;
    // Update는 매 프레임 돌지만, if문 하나 체크하는 건 CPU 비용이 거의 0에 가깝습니다.
    private void Update()
    {
        // 1. 기다리는 상태가 아니라면 즉시 리턴 (성능 부하 없음)
        if (!isWaitingForInput) return;

    }
    private void Start()
    {
        InputSystem.Instance.RegisterAction(KeyState.Play_Key, KeyCode.Mouse0, ResumeTimeline);
        InputSystem.Instance.RegisterAction(KeyState.Play_Key, KeyCode.Space, ResumeTimeline);
    }
    // [Signal Receiver에 연결할 함수]
    public void PauseAndWaitForInput()
    {
        if (director != null)
        {

            director.playableGraph.GetRootPlayable(0).SetSpeed(0);
            isWaitingForInput = true;
        }
    }

    private void ResumeTimeline()
    {
        if ((float)director.time <= 1f)
            return;
        if (director != null)
        {
            // 1. 타임라인 재개
            director.playableGraph.GetRootPlayable(0).SetSpeed(1);
            isWaitingForInput = false;
        }
    }

    public void SetTimelineSpeed(float speed)
    {
        if (director.playableGraph.IsValid())
        {
            // 타임라인의 재생 속도를 직접 조절
            director.playableGraph.GetRootPlayable(0).SetSpeed(speed);
        }
    }
    public void SendNextScene(string name)
    {
        SceneManager.LoadScene(name);
    }
}

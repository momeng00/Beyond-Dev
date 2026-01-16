using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class TimeLineController : MonoBehaviour
{
    public PlayableDirector director;
    // 현재 입력을 기다리는 중인가?
    private bool isWaitingForInput = false;
    [SerializeField] private bool EndingEnd = false;

    // Update는 매 프레임 돌지만, if문 하나 체크하는 건 CPU 비용이 거의 0에 가깝습니다.
    private void Update()
    {
        if (isWaitingForInput) return;
        if (director.state != PlayState.Playing) return;
        if (!director.playableGraph.IsValid()) return;

        // 1. 목표 속도 결정 (누르고 있으면 3배, 아니면 1배)
        // GetKey를 써서 Alt+Tab 문제 원천 차단
        float targetSpeed = Input.GetKey(KeyCode.Space) ? 3.0f : 1.0f;

        // 2. [최적화] 현재 속도와 목표 속도가 다를 때만! 변경함
        // 이러면 매 프레임 호출되는 낭비를 막을 수 있음
        double currentSpeed = director.playableGraph.GetRootPlayable(0).GetSpeed();

        if (Mathf.Abs((float)currentSpeed - targetSpeed) > 0.01f) // 부동소수점 오차 고려
        {
            SetTimelineSpeed(targetSpeed);

            // UI 켜고 끄기
        }
        // 1. 기다리는 상태가 아니라면 즉시 리턴 (성능 부하 없음)
        if (EndingEnd)
        {
            if (Input.anyKeyDown)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
        }
            
    }
    private void Start()
    {

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
            isWaitingForInput = false;
            director.playableGraph.GetRootPlayable(0).SetSpeed(speed);
        }
    }
    public void SendNextScene(string name)
    {
        SceneManager.LoadScene(name);
    }
    public void EndGame()
    {
        EndingEnd = true;
    }
}

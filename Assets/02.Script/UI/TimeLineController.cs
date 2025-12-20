using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

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

        // 2. 기다리는 상태일 때만 입력 체크
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ResumeTimeline();
        }
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
        if (director != null)
        {
            // 1. 타임라인 재개
            director.playableGraph.GetRootPlayable(0).SetSpeed(1);
            isWaitingForInput = false;
        }
    }
}

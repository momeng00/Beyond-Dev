using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISequenceController : MonoBehaviour
{
    [System.Serializable]
    public struct SequenceStep
    {
        public UIBase targetUI; // 실행할 UI
        [Tooltip("다음 UI가 열리기 전 대기 시간")]
        public float delayNext; // 대기 시간
    }

    public bool playOnStart = false;
    public List<SequenceStep> sequenceList = new List<SequenceStep>();

    private void Start()
    {
        if (playOnStart) Play();
    }

    public void Play()
    {
        StopAllCoroutines();
        StartCoroutine(ProcessSequence());
    }

    IEnumerator ProcessSequence()
    {
        // [1단계] 초기화 페이즈: 모든 UI를 켜고, 시작 위치로 강제 이동시킴
        // 이렇게 하면 "제자리에 있다가 이동하는" 현상이 사라짐 (문제 1번 해결)
        foreach (var step in sequenceList)
        {
            if (step.targetUI != null)
            {
                // 미리 켜두되, 화면 밖이나 크기 0으로 설정해둠
                step.targetUI.SetInitialState();
            }
        }

        // 한 프레임 대기 (RectTransform 갱신 등 안정성 확보)
        yield return null;

        // [2단계] 실행 페이즈: 하나씩 순서대로 Open 호출
        foreach (var step in sequenceList)
        {
            if (step.targetUI != null)
            {
                // 이미 SetInitialState에서 켜져(Active)있으므로 코루틴 오류 안 남
                step.targetUI.Open();
            }

            if (step.delayNext > 0)
            {
                yield return new WaitForSeconds(step.delayNext);
            }
        }
    }
}
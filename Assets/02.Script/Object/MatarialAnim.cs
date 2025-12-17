using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatarialAnim
{
    public enum AnimMode
    {
        OneShot, // 한 번만 실행하고 멈춤 
        Loop     // 계속 처음부터 다시 시작 
    }

    [System.Serializable]
    public struct PropertyData
    {
        [Tooltip("제어할 쉐이더 프로퍼티 이름 (예: _Cutoff, _Emission)")]
        public string propertyName;

        [Tooltip("시작 값")]
        public float startValue;

        [Tooltip("끝 값")]
        public float endValue;

        [Tooltip("작동 방식 선택")]
        public AnimMode mode;
    }

    // 문자열 검색 비용을 줄이기 위해 int ID로 변환하여 저장할 구조체
    private struct CachedData
    {
        public int propertyID;
        public float startValue;
        public float endValue;
    }

    private struct ReturnData
    {
        public int propertyID;
        public float currentVal; // 멈춘 시점의 값
        public float targetVal;  // 원래 시작 값 (StartValue)
    }

    //복사될 데이터
    private List<CachedData> cachedOneShotList = new List<CachedData>();
    private List<CachedData> cachedLoopList = new List<CachedData>();
    [Tooltip("애니메이션이 걸리는 시간 (초)")]
    public float duration;
    private MonoBehaviour runner;
    private Material targetMaterial;
    //원하는 곳으로 집어 넣기
    public void InitMatarialAnim(MonoBehaviour runner, Material material, List<PropertyData> rawData, float Duration)
    {
        this.runner = runner;
        this.targetMaterial = material;
        cachedOneShotList.Clear();
        cachedLoopList.Clear();

        duration = Duration;
        foreach (var data in rawData)
        {
            CachedData cache = new CachedData
            {
                propertyID = Shader.PropertyToID(data.propertyName),
                startValue = data.startValue,
                endValue = data.endValue
            };

            if (data.mode == AnimMode.OneShot)
            {
                cachedOneShotList.Add(cache);
            }
            else if (data.mode == AnimMode.Loop)
            {
                cachedLoopList.Add(cache);
            }
        }
    }


    private Coroutine activeOneShot;
    private Coroutine activeLoop;
    private Coroutine activeReturn;
    public void Play()
    {
        Stop(); // 기존 코루틴 정리

        if (runner == null || targetMaterial == null) return;

        // OneShot 실행
        if (cachedOneShotList.Count > 0)
            activeOneShot = runner.StartCoroutine(OneShotRoutine());

        // Loop 실행
        if (cachedLoopList.Count > 0)
            activeLoop = runner.StartCoroutine(LoopRoutine());
    }

    // --- [3. 정지 및 초기화 함수] ---
    public void Stop()
    {
        if (runner != null)
        {
            if (activeOneShot != null) runner.StopCoroutine(activeOneShot);
            if (activeLoop != null) runner.StopCoroutine(activeLoop);
        }

        // 멈출 때 초기값(StartValue)으로 되돌릴지, 
        // 끝값(EndValue)으로 둘지는 기획 의도에 따라 여기서 설정
        ResetToStartValues();
    }
    public void PlayReturn()
    {
        // 1. 진행 중인 모든 애니메이션 중단
        StopAll();

        // 2. 돌아가는 코루틴 시작
        if (runner != null && targetMaterial != null)
        {
            activeReturn = runner.StartCoroutine(ReturnRoutine());
        }
    }

    // 내부적으로 모든 코루틴 멈추기
    private void StopAll()
    {
        if (runner != null)
        {
            if (activeOneShot != null) runner.StopCoroutine(activeOneShot);
            if (activeLoop != null) runner.StopCoroutine(activeLoop);
            if (activeReturn != null) runner.StopCoroutine(activeReturn);
        }
        activeOneShot = null;
        activeLoop = null;
        activeReturn = null;
    }

    // --- [추가된 코루틴] 돌아가기 로직 ---
    private IEnumerator ReturnRoutine()
    {
        // 1. [스냅샷] 현재 멈춘 시점의 값들을 다 기록함
        List<ReturnData> snapshotList = new List<ReturnData>();

        // OneShot 리스트 현재 값 캡처
        CaptureCurrentState(cachedOneShotList, snapshotList);
        // Loop 리스트 현재 값 캡처
        CaptureCurrentState(cachedLoopList, snapshotList);

        float timer = 0f;

        // 2. 현재 값 -> StartValue로 선형 이동
        while (timer < duration)
        {
            timer += Time.deltaTime;
            float t = Mathf.Clamp01(timer / duration);

            foreach (var data in snapshotList)
            {
                // 멈춘 값(Current)에서 -> 원래 시작 값(Target)으로 보간
                float val = Mathf.Lerp(data.currentVal, data.targetVal, t);
                targetMaterial.SetFloat(data.propertyID, val);
            }

            yield return null;
        }

        // 3. 확실하게 원점(StartValue)으로 고정
        foreach (var data in snapshotList)
        {
            targetMaterial.SetFloat(data.propertyID, data.targetVal);
        }
    }

    // 현재 머티리얼의 값을 읽어와서 리스트에 담는 헬퍼 함수
    private void CaptureCurrentState(List<CachedData> sourceList, List<ReturnData> targetList)
    {
        foreach (var item in sourceList)
        {
            ReturnData rData = new ReturnData
            {
                propertyID = item.propertyID,
                // [중요] 현재 머티리얼이 가진 값을 읽어옴 (GetFloat)
                currentVal = targetMaterial.GetFloat(item.propertyID),
                // 목표는 원래 설정된 StartValue
                targetVal = item.startValue
            };
            targetList.Add(rData);
        }
    }
    private void ResetToStartValues()
    {
        if (targetMaterial == null) return;

        // 모든 리스트의 값을 StartValue로 초기화
        ApplyValues(cachedOneShotList, 0f); // t=0 (Start)
        ApplyValues(cachedLoopList, 0f);    // t=0 (Start)
    }

    private IEnumerator OneShotRoutine()
    {
        float timer = 0f;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            float t = Mathf.Clamp01(timer / duration); // 0 ~ 1 선형 비율

            ApplyValues(cachedOneShotList, t);
            yield return null;
        }
        // 확실한 마무리
        ApplyValues(cachedOneShotList, 1f);
    }

    // 계속 반복되는 로직
    private IEnumerator LoopRoutine()
    {
        while (true)
        {
            float timer = 0f;
            while (timer < duration)
            {
                timer += Time.deltaTime;
                float t = Mathf.Clamp01(timer / duration); // 0 ~ 1 선형 비율

                ApplyValues(cachedLoopList, t);
                yield return null;
            }

            // Loop이므로 다시 처음(0)부터 시작하기 위해 
            // 여기서는 끝값 유지 없이 바로 다음 프레임에 0으로 돌아감
            // (만약 핑퐁을 원하면 여기에 로직 추가)
        }
    }


    private void ApplyValues(List<CachedData> list, float t)
    {
        for (int i = 0; i < list.Count; i++)
        {
            CachedData data = list[i];
            // 선형 보간 (Lerp)
            float value = Mathf.Lerp(data.startValue, data.endValue, t);
            targetMaterial.SetFloat(data.propertyID, value);
        }
    }
}
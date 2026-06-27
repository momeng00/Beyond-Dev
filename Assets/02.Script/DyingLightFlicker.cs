using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(Light2D))]
public class DyingLightFlicker : MonoBehaviour
{
    [Header("기본 밝기")]
    [Tooltip("체크하면 시작 시 Light2D에 설정된 Intensity를 사용합니다.")]
    [SerializeField] private bool useCurrentIntensity = true;

    [SerializeField, Min(0f)]
    private float normalIntensity = 1f;

    [Header("정상적으로 켜져 있는 시간")]
    [SerializeField]
    private Vector2 stableTimeRange = new Vector2(1.5f, 6f);

    [Header("한 번에 깜빡이는 횟수")]
    [SerializeField]
    private Vector2Int flickerCountRange = new Vector2Int(2, 6);

    [Header("짧게 어두워지는 시간")]
    [SerializeField]
    private Vector2 darkTimeRange = new Vector2(0.025f, 0.12f);

    [Header("다시 켜지는 시간")]
    [SerializeField]
    private Vector2 litTimeRange = new Vector2(0.03f, 0.18f);

    [Header("어두워졌을 때 밝기 비율")]
    [SerializeField]
    private Vector2 dimIntensityRatio = new Vector2(0.05f, 0.35f);

    [Header("고장 표현")]
    [SerializeField, Range(0f, 1f)]
    private float blackoutChance = 0.65f;

    [Tooltip("가끔 비교적 오래 꺼져 있을 확률")]
    [SerializeField, Range(0f, 1f)]
    private float longBlackoutChance = 0.12f;

    [SerializeField]
    private Vector2 longBlackoutTimeRange = new Vector2(0.2f, 0.7f);

    [Header("다시 켜질 때 밝기 흔들림")]
    [SerializeField]
    private Vector2 recoveryIntensityRatio = new Vector2(0.85f, 1.05f);

    private Light2D targetLight;
    private Coroutine flickerRoutine;
    private float baseIntensity;

    private void OnEnable()
    {
        targetLight = GetComponent<Light2D>();

        baseIntensity = useCurrentIntensity
            ? targetLight.intensity
            : normalIntensity;

        flickerRoutine = StartCoroutine(FlickerLoop());
    }

    private void OnDisable()
    {
        if (flickerRoutine != null)
        {
            StopCoroutine(flickerRoutine);
            flickerRoutine = null;
        }

        if (targetLight != null)
            targetLight.intensity = baseIntensity;
    }

    private IEnumerator FlickerLoop()
    {
        while (true)
        {
            // 대부분의 시간은 정상적으로 켜져 있음
            targetLight.intensity = baseIntensity;

            yield return new WaitForSeconds(
                GetRandomValue(stableTimeRange)
            );

            int minCount = Mathf.Max(
                1,
                Mathf.Min(flickerCountRange.x, flickerCountRange.y)
            );

            int maxCount = Mathf.Max(
                minCount,
                Mathf.Max(flickerCountRange.x, flickerCountRange.y)
            );

            int flickerCount = Random.Range(minCount, maxCount + 1);

            // 짧은 점멸이 연속해서 발생
            for (int i = 0; i < flickerCount; i++)
            {
                bool completeBlackout =
                    Random.value < blackoutChance;

                float dimRatio = completeBlackout
                    ? 0f
                    : GetRandomValue(dimIntensityRatio);

                targetLight.intensity = baseIntensity * dimRatio;

                float darkTime = GetRandomValue(darkTimeRange);

                if (Random.value < longBlackoutChance)
                    darkTime = GetRandomValue(longBlackoutTimeRange);

                yield return new WaitForSeconds(darkTime);

                targetLight.intensity =
                    baseIntensity *
                    GetRandomValue(recoveryIntensityRatio);

                yield return new WaitForSeconds(
                    GetRandomValue(litTimeRange)
                );
            }
        }
    }

    private static float GetRandomValue(Vector2 range)
    {
        float min = Mathf.Min(range.x, range.y);
        float max = Mathf.Max(range.x, range.y);

        return Random.Range(min, max);
    }
}
using UnityEngine;
using Unity.Cinemachine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteAlways]
[DefaultExecutionOrder(10000)]
[RequireComponent(typeof(CinemachineBrain))]
public sealed class CinemachineFrameLimiter : MonoBehaviour
{
    [SerializeField, Min(1f)]
    private float cameraFrameRate = 24f;

    [SerializeField]
    private bool runInEditMode = true;

    private CinemachineBrain brain;

    private CinemachineBrain.UpdateMethods previousUpdateMethod;
    private bool previousUpdateMethodStored;

    private float accumulatedTime;

#if UNITY_EDITOR
    private double previousEditorTime;
#endif

    private void OnEnable()
    {
        brain = GetComponent<CinemachineBrain>();

        if (brain == null)
            return;

        previousUpdateMethod = brain.UpdateMethod;
        previousUpdateMethodStored = true;

        brain.UpdateMethod =
            CinemachineBrain.UpdateMethods.ManualUpdate;

        ResetTiming();

#if UNITY_EDITOR
        EditorApplication.update -= EditorUpdate;
        EditorApplication.update += EditorUpdate;
#endif
    }

    private void OnDisable()
    {
#if UNITY_EDITOR
        EditorApplication.update -= EditorUpdate;
#endif

        CinemachineCore.UniformDeltaTimeOverride = -1f;

        if (brain != null && previousUpdateMethodStored)
            brain.UpdateMethod = previousUpdateMethod;
    }

    private void LateUpdate()
    {
        if (!Application.isPlaying)
            return;

        Tick(Time.unscaledDeltaTime);
    }

#if UNITY_EDITOR
    private void EditorUpdate()
    {
        double currentEditorTime =
            EditorApplication.timeSinceStartup;

        float deltaTime =
            (float)(currentEditorTime - previousEditorTime);

        previousEditorTime = currentEditorTime;

        if (Application.isPlaying)
            return;

        if (!runInEditMode)
            return;

        if (!isActiveAndEnabled || brain == null)
            return;

        // 에디터가 멈춰 있었을 때 지나치게 큰 deltaTime이
        // 들어가는 것을 방지한다.
        deltaTime = Mathf.Clamp(deltaTime, 0f, 0.1f);

        if (!Tick(deltaTime))
            return;

        // Game View와 Scene View가 새 카메라 위치를 다시 그리도록 한다.
        EditorApplication.QueuePlayerLoopUpdate();
        SceneView.RepaintAll();
    }
#endif

    private bool Tick(float deltaTime)
    {
        float frameInterval = 1f / cameraFrameRate;

        accumulatedTime += Mathf.Max(0f, deltaTime);

        if (accumulatedTime < frameInterval)
            return false;

        accumulatedTime %= frameInterval;

        CinemachineCore.UniformDeltaTimeOverride =
            frameInterval;

        try
        {
            // UpdateMethod가 ManualUpdate일 때 외부에서 명시적으로
            // 호출하여 가상 카메라와 실제 카메라를 갱신한다.
            brain.ManualUpdate();
        }
        finally
        {
            CinemachineCore.UniformDeltaTimeOverride = -1f;
        }

        return true;
    }

    private void ResetTiming()
    {
        accumulatedTime = 0f;

#if UNITY_EDITOR
        previousEditorTime =
            EditorApplication.timeSinceStartup;
#endif
    }

    private void OnValidate()
    {
        cameraFrameRate =
            Mathf.Max(1f, cameraFrameRate);

        ResetTiming();
    }
}
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Rendering;

[ExecuteAlways]
[DisallowMultipleComponent]
[DefaultExecutionOrder(10000)]
[AddComponentMenu("Animation/Timed Position Constraint")]
public sealed class TimedPositionConstraint : MonoBehaviour, IConstraint
{
    public enum UpdateTiming
    {
        Update,
        LateUpdate,
        FixedUpdate,
        BeforeRender,
        Manual
    }

    [Serializable]
    public struct Source
    {
        [InspectorName("Source")]
        public Transform sourceTransform;

        [Range(0f, 1f)]
        public float weight;

        public Source(
            Transform sourceTransform,
            float weight = 1f)
        {
            this.sourceTransform = sourceTransform;
            this.weight = weight;
        }
    }

    [SerializeField]
    private UpdateTiming m_UpdateTiming =
        UpdateTiming.LateUpdate;

    [SerializeField]
    private Camera m_RenderCamera;

    [SerializeField]
    private bool m_ConstraintActive;

    [SerializeField, Range(0f, 1f)]
    private float m_Weight = 1f;

    [SerializeField]
    private bool m_Locked;

    [SerializeField]
    private Vector3 m_TranslationAtRest;

    [SerializeField]
    private Vector3 m_TranslationOffset;

    [SerializeField]
    private Axis m_TranslationAxis =
        Axis.X | Axis.Y | Axis.Z;

    [SerializeField]
    private List<Source> m_Sources = new();

    [NonSerialized]
    private bool m_EditorSnapshotValid;

    [NonSerialized]
    private Vector3 m_LastLocalPosition;

    [NonSerialized]
    private int m_LastSourceStateHash;

    public UpdateTiming updateTiming
    {
        get => m_UpdateTiming;
        set => m_UpdateTiming = value;
    }

    public Camera renderCamera
    {
        get => m_RenderCamera;
        set => m_RenderCamera = value;
    }

    public bool constraintActive
    {
        get => m_ConstraintActive;
        set => m_ConstraintActive = value;
    }

    public float weight
    {
        get => m_Weight;
        set => m_Weight = Mathf.Clamp01(value);
    }

    public bool locked
    {
        get => m_Locked;
        set => m_Locked = value;
    }

    public Vector3 translationAtRest
    {
        get => m_TranslationAtRest;
        set => m_TranslationAtRest = value;
    }

    public Vector3 translationOffset
    {
        get => m_TranslationOffset;
        set => m_TranslationOffset = value;
    }

    public Axis translationAxis
    {
        get => m_TranslationAxis;
        set => m_TranslationAxis = value;
    }

    public int sourceCount => m_Sources.Count;

    private void Reset()
    {
        m_TranslationAtRest = transform.localPosition;
        CaptureEditorSnapshot();
    }

    private void OnEnable()
    {
        // 도메인 리로드 등으로 중복 등록되는 상황 방지
        RenderPipelineManager.beginCameraRendering -=
            OnBeginCameraRendering;

        RenderPipelineManager.beginCameraRendering +=
            OnBeginCameraRendering;

        CaptureEditorSnapshot();
    }

    private void OnDisable()
    {
        RenderPipelineManager.beginCameraRendering -=
            OnBeginCameraRendering;
    }

    private void OnDestroy()
    {
        RenderPipelineManager.beginCameraRendering -=
            OnBeginCameraRendering;
    }

    private void OnValidate()
    {
        m_Weight = Mathf.Clamp01(m_Weight);

        for (int i = 0; i < m_Sources.Count; i++)
        {
            Source source = m_Sources[i];
            source.weight = Mathf.Clamp01(source.weight);
            m_Sources[i] = source;
        }

        m_EditorSnapshotValid = false;
    }

    private void Update()
    {
        if (!Application.isPlaying)
        {
            EditorUpdate();
            return;
        }

        if (m_UpdateTiming == UpdateTiming.Update)
            EvaluateNow();
    }

    private void LateUpdate()
    {
        if (!Application.isPlaying)
            return;

        if (m_UpdateTiming == UpdateTiming.LateUpdate)
            EvaluateNow();
    }

    private void FixedUpdate()
    {
        if (!Application.isPlaying)
            return;

        if (m_UpdateTiming == UpdateTiming.FixedUpdate)
            EvaluateNow();
    }

    private void OnBeginCameraRendering(
        ScriptableRenderContext context,
        Camera renderingCamera)
    {
        if (m_UpdateTiming != UpdateTiming.BeforeRender)
            return;

        if (!m_ConstraintActive)
            return;

        Camera targetCamera = ResolveRenderCamera();

        if (targetCamera == null)
            return;

        if (renderingCamera != targetCamera)
            return;

        EvaluateNow();
    }

    private Camera ResolveRenderCamera()
    {
        if (m_RenderCamera != null)
            return m_RenderCamera;

        // Render Camera가 비어 있으면
        // Sources에 들어 있는 Camera를 우선 사용한다.
        for (int i = 0; i < m_Sources.Count; i++)
        {
            Transform sourceTransform =
                m_Sources[i].sourceTransform;

            if (sourceTransform == null)
                continue;

            if (sourceTransform.TryGetComponent(
                    out Camera sourceCamera))
            {
                return sourceCamera;
            }
        }

        return Camera.main;
    }

    private void EditorUpdate()
    {
        if (!m_ConstraintActive)
        {
            CaptureEditorSnapshot();
            return;
        }

        if (!m_Locked)
        {
            UpdateUnlockedOffset();
            return;
        }

        // BeforeRender는 렌더 파이프라인 콜백에서 평가한다.
        if (m_UpdateTiming == UpdateTiming.BeforeRender)
            return;

        if (m_UpdateTiming != UpdateTiming.Manual)
            EvaluateNow();
    }

    public void EvaluateNow()
    {
        if (!m_ConstraintActive)
            return;

        // 원본 Position Constraint처럼
        // 에디터에서 Lock이 꺼져 있으면 오브젝트 이동을 허용한다.
        if (!Application.isPlaying && !m_Locked)
            return;

        transform.localPosition =
            CalculateConstrainedLocalPosition();

        CaptureEditorSnapshot();
    }

    private Vector3 CalculateConstrainedLocalPosition()
    {
        Vector3 restPosition = m_TranslationAtRest;
        Vector3 sourcePosition = restPosition;

        if (TryGetWeightedSourcePosition(
                out Vector3 weightedSourceLocalPosition,
                out float totalSourceWeight))
        {
            float sourceInfluence =
                Mathf.Clamp01(totalSourceWeight);

            Vector3 sourceWithOffset =
                weightedSourceLocalPosition +
                m_TranslationOffset;

            /*
             * 현재 transform.position을 기준으로 보간하지 않는다.
             *
             * 따라서 프레임마다 목표를 뒤늦게 추적하는
             * 누적 Lerp/댐핑이 발생하지 않는다.
             */
            sourcePosition = Vector3.LerpUnclamped(
                restPosition,
                sourceWithOffset,
                sourceInfluence
            );
        }

        Vector3 result = Vector3.LerpUnclamped(
            restPosition,
            sourcePosition,
            Mathf.Clamp01(m_Weight)
        );

        // Constraint가 적용되지 않는 축은 At Rest 값을 사용한다.
        if ((m_TranslationAxis & Axis.X) == 0)
            result.x = restPosition.x;

        if ((m_TranslationAxis & Axis.Y) == 0)
            result.y = restPosition.y;

        if ((m_TranslationAxis & Axis.Z) == 0)
            result.z = restPosition.z;

        return result;
    }

    private bool TryGetWeightedSourcePosition(
        out Vector3 localPosition,
        out float totalWeight)
    {
        Vector3 weightedWorldPosition = Vector3.zero;
        totalWeight = 0f;

        for (int i = 0; i < m_Sources.Count; i++)
        {
            Source source = m_Sources[i];

            if (source.sourceTransform == null)
                continue;

            float sourceWeight =
                Mathf.Clamp01(source.weight);

            if (sourceWeight <= 0f)
                continue;

            weightedWorldPosition +=
                source.sourceTransform.position *
                sourceWeight;

            totalWeight += sourceWeight;
        }

        if (totalWeight <= Mathf.Epsilon)
        {
            localPosition = m_TranslationAtRest;
            return false;
        }

        Vector3 averagedWorldPosition =
            weightedWorldPosition / totalWeight;

        Transform parent = transform.parent;

        localPosition = parent != null
            ? parent.InverseTransformPoint(
                averagedWorldPosition)
            : averagedWorldPosition;

        return true;
    }

    public void Activate()
    {
        Vector3 currentLocalPosition =
            transform.localPosition;

        m_TranslationAtRest =
            currentLocalPosition;

        if (TryGetWeightedSourcePosition(
                out Vector3 sourceLocalPosition,
                out _))
        {
            m_TranslationOffset =
                currentLocalPosition -
                sourceLocalPosition;
        }
        else
        {
            m_TranslationOffset =
                Vector3.zero;
        }

        m_ConstraintActive = true;
        m_Locked = true;

        EvaluateNow();
        CaptureEditorSnapshot();
    }

    public void Zero()
    {
        m_TranslationAtRest = Vector3.zero;
        m_TranslationOffset = Vector3.zero;

        m_ConstraintActive = true;
        m_Locked = true;

        EvaluateNow();
        CaptureEditorSnapshot();
    }

    private void UpdateUnlockedOffset()
    {
        Vector3 currentLocalPosition =
            transform.localPosition;

        int sourceStateHash =
            CalculateSourceStateHash();

        if (!m_EditorSnapshotValid)
        {
            CaptureEditorSnapshot();
            return;
        }

        bool objectMoved =
            currentLocalPosition != m_LastLocalPosition;

        bool sourceChanged =
            sourceStateHash != m_LastSourceStateHash;

        if (!objectMoved && !sourceChanged)
            return;

        m_TranslationAtRest =
            currentLocalPosition;

        if (TryGetWeightedSourcePosition(
                out Vector3 sourceLocalPosition,
                out _))
        {
            m_TranslationOffset =
                currentLocalPosition -
                sourceLocalPosition;
        }

        CaptureEditorSnapshot();
    }

    private void CaptureEditorSnapshot()
    {
        m_LastLocalPosition =
            transform.localPosition;

        m_LastSourceStateHash =
            CalculateSourceStateHash();

        m_EditorSnapshotValid = true;
    }

    private int CalculateSourceStateHash()
    {
        unchecked
        {
            int hash = 17;

            for (int i = 0; i < m_Sources.Count; i++)
            {
                Source source = m_Sources[i];

                hash =
                    hash * 31 +
                    source.weight.GetHashCode();

                if (source.sourceTransform != null)
                {
                    hash =
                        hash * 31 +
                        source.sourceTransform
                            .position
                            .GetHashCode();
                }
            }

            if (transform.parent != null)
            {
                hash =
                    hash * 31 +
                    transform.parent
                        .localToWorldMatrix
                        .GetHashCode();
            }

            return hash;
        }
    }

    public int AddSource(ConstraintSource source)
    {
        m_Sources.Add(
            new Source(
                source.sourceTransform,
                source.weight
            )
        );

        return m_Sources.Count - 1;
    }

    public void RemoveSource(int index)
    {
        ValidateSourceIndex(index);
        m_Sources.RemoveAt(index);
    }

    public ConstraintSource GetSource(int index)
    {
        ValidateSourceIndex(index);

        Source source = m_Sources[index];

        return new ConstraintSource
        {
            sourceTransform =
                source.sourceTransform,

            weight =
                source.weight
        };
    }

    public void SetSource(
        int index,
        ConstraintSource source)
    {
        ValidateSourceIndex(index);

        m_Sources[index] =
            new Source(
                source.sourceTransform,
                source.weight
            );
    }

    public void GetSources(
        List<ConstraintSource> sources)
    {
        if (sources == null)
        {
            throw new ArgumentNullException(
                nameof(sources)
            );
        }

        sources.Clear();

        for (int i = 0; i < m_Sources.Count; i++)
        {
            Source source = m_Sources[i];

            sources.Add(
                new ConstraintSource
                {
                    sourceTransform =
                        source.sourceTransform,

                    weight =
                        source.weight
                }
            );
        }
    }

    public void SetSources(
        List<ConstraintSource> sources)
    {
        if (sources == null)
        {
            throw new ArgumentNullException(
                nameof(sources)
            );
        }

        m_Sources.Clear();

        for (int i = 0; i < sources.Count; i++)
        {
            ConstraintSource source = sources[i];

            m_Sources.Add(
                new Source(
                    source.sourceTransform,
                    source.weight
                )
            );
        }
    }

    private void ValidateSourceIndex(int index)
    {
        if (m_Sources.Count == 0)
        {
            throw new InvalidOperationException(
                "The constraint has no sources."
            );
        }

        if (index < 0 ||
            index >= m_Sources.Count)
        {
            throw new ArgumentOutOfRangeException(
                nameof(index),
                $"Source index {index} is outside " +
                $"0–{m_Sources.Count - 1}."
            );
        }
    }
}
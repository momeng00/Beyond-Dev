using UnityEngine;

[DisallowMultipleComponent]
public class CameraVisibility : MonoBehaviour
{
    [Tooltip("월드 오브젝트를 검사할 카메라입니다. 비워두면 메인 카메라를 사용하고, 카메라 방식 UI는 Canvas에 지정된 카메라를 사용합니다.")]
    [SerializeField] private Camera visibilityCamera;
    [Min(0f)] [SerializeField] private float visibilityPadding = 0.1f;

    [Header("Detection Area")]
    [Tooltip("감지 영역의 기준 Transform입니다. 비워두면 자기 자신을 사용합니다. 표시 대상을 숨겨도 감지 컴포넌트는 활성 상태를 유지해야 합니다.")]
    [SerializeField] private Transform detectionOrigin;
    [Tooltip("저장된 고정 영역을 사용합니다. Renderer나 RectTransform의 크기 변화를 자동 반영하려면 끄세요. 컴포넌트 메뉴의 Bake Detection Bounds를 실행하면 현재 영역이 저장됩니다.")]
    [SerializeField] private bool useStoredBounds;
    [SerializeField] private Bounds storedLocalBounds = new Bounds(Vector3.zero, Vector3.one);

    private Transform sourceTransform;
    private Transform sourceParent;
    private Renderer targetRenderer;
    private RectTransform targetRect;
    private Canvas targetCanvas;
    private bool initialized;
    private bool worldBoundsValid;
    private Bounds previousLocalBounds;
    private Matrix4x4 previousLocalToWorld;
    private Bounds worldBounds;
    private bool paddingScaleValid;
    private Matrix4x4 previousEffectMatrix;
    private Vector3 paddingPerLocalUnit;

    private void Awake()
    {
        EnsureSource();
    }

    private void OnTransformParentChanged()
    {
        RefreshBounds();
    }

    private void OnCanvasHierarchyChanged()
    {
        RefreshBounds();
    }

    private void OnValidate()
    {
        visibilityPadding = Mathf.Max(0f, visibilityPadding);
        Vector3 size = storedLocalBounds.size;
        storedLocalBounds.size = new Vector3(Mathf.Abs(size.x), Mathf.Abs(size.y), Mathf.Abs(size.z));
        RefreshBounds();
    }

    // Use after adding/replacing source components at runtime.
    public void RefreshBounds()
    {
        initialized = false;
        worldBoundsValid = false;
        paddingScaleValid = false;
    }

    private void EnsureSource()
    {
        Transform origin = detectionOrigin;
        if (origin == null)
            origin = transform;

        if (initialized && sourceTransform == origin && sourceParent == origin.parent)
            return;

        sourceTransform = origin;
        sourceParent = origin.parent;
        targetRenderer = origin.GetComponent<Renderer>();
        targetRect = origin as RectTransform;
        targetCanvas = origin.GetComponentInParent<Canvas>();
        worldBoundsValid = false;
        initialized = true;
    }

    [ContextMenu("Bake Detection Bounds")]
    public void BakeDetectionBounds()
    {
        EnsureSource();
        if (!TryGetAutomaticLocalBounds(out Bounds bounds))
        {
            Debug.LogWarning("No Renderer or RectTransform to bake. Enable Use Stored Bounds and enter an area manually.", this);
            return;
        }

#if UNITY_EDITOR
        if (!Application.isPlaying)
            UnityEditor.Undo.RecordObject(this, "Bake Detection Bounds");
#endif
        storedLocalBounds = bounds;
        useStoredBounds = true;
        RefreshBounds();

#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            UnityEditor.EditorUtility.SetDirty(this);
            if (UnityEditor.PrefabUtility.IsPartOfPrefabInstance(this))
                UnityEditor.PrefabUtility.RecordPrefabInstancePropertyModifications(this);
            if (gameObject.scene.IsValid() && gameObject.scene.isLoaded)
                UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(gameObject.scene);
        }
#endif
    }

    // Queries run only when a consumer needs them; this component has no update loop.
    public bool IsVisible(float additionalLocalPadding = 0f, Camera cameraOverride = null)
    {
        if (!isActiveAndEnabled)
            return false;

        EnsureSource();

        if (targetCanvas != null && !targetCanvas.isActiveAndEnabled)
            return false;

        if (!TryGetWorldBounds(out Bounds bounds))
            return false;

        Vector3 margin = Vector3.one * Mathf.Max(0f, visibilityPadding);
        if (additionalLocalPadding > 0f)
            margin += GetPaddingPerLocalUnit() * additionalLocalPadding;
        bounds.Expand(margin * 2f);

        Camera camera = cameraOverride;
        if (camera == null)
            camera = visibilityCamera;
        if (targetCanvas != null)
        {
            Canvas rootCanvas = targetCanvas.rootCanvas;
            if (!rootCanvas.isActiveAndEnabled)
                return false;

            bool usesScreenCoordinates = rootCanvas.renderMode == RenderMode.ScreenSpaceOverlay;
            if (rootCanvas.renderMode == RenderMode.ScreenSpaceCamera && rootCanvas.worldCamera == null)
                usesScreenCoordinates = true;

            if (usesScreenCoordinates)
            {
                Rect screen = rootCanvas.pixelRect;
                return bounds.max.x >= screen.xMin && bounds.min.x <= screen.xMax &&
                       bounds.max.y >= screen.yMin && bounds.min.y <= screen.yMax;
            }

            if (rootCanvas.renderMode == RenderMode.ScreenSpaceCamera && rootCanvas.worldCamera != null)
                camera = rootCanvas.worldCamera;
        }

        return CameraVisibilityUtility.IsInGameCameraView(bounds, sourceTransform.gameObject.layer, camera);
    }

    private bool TryGetAutomaticLocalBounds(out Bounds bounds)
    {
        // Keep the layout area even with no glyphs, and include overflowing world text.
        bool hasBounds = false;
        bounds = default;
        if (targetRect != null)
        {
            Rect rect = targetRect.rect;
            bounds = new Bounds(new Vector3(rect.center.x, rect.center.y, 0f),
                new Vector3(rect.width, rect.height, 0f));
            hasBounds = true;
        }

        if (targetRenderer != null)
        {
            Bounds rendererBounds = targetRenderer.localBounds;
            if (hasBounds)
                bounds.Encapsulate(rendererBounds);
            else
                bounds = rendererBounds;
            hasBounds = true;
        }

        return hasBounds;
    }

    private bool TryGetWorldBounds(out Bounds bounds)
    {
        Bounds localBounds = storedLocalBounds;
        if (!useStoredBounds && !TryGetAutomaticLocalBounds(out localBounds))
        {
            bounds = default;
            return false;
        }

        // Matrix comparison includes parent movement/rotation/scale without resetting Transform.hasChanged.
        Matrix4x4 localToWorld = sourceTransform.localToWorldMatrix;
        if (!worldBoundsValid || !previousLocalBounds.Equals(localBounds) ||
            !previousLocalToWorld.Equals(localToWorld))
        {
            Vector3 center = localToWorld.MultiplyPoint3x4(localBounds.center);
            Vector3 extents = TransformExtents(localToWorld, localBounds.extents);
            worldBounds = new Bounds(center, extents * 2f);
            previousLocalBounds = localBounds;
            previousLocalToWorld = localToWorld;
            worldBoundsValid = true;
        }

        bounds = worldBounds;
        return true;
    }

    private Vector3 GetPaddingPerLocalUnit()
    {
        // Effect displacement is expressed in the consumer object's local XY space.
        Matrix4x4 effectMatrix = transform.localToWorldMatrix;
        if (!paddingScaleValid || !previousEffectMatrix.Equals(effectMatrix))
        {
            paddingPerLocalUnit = TransformExtents(effectMatrix, new Vector3(1f, 1f, 0f));
            previousEffectMatrix = effectMatrix;
            paddingScaleValid = true;
        }
        return paddingPerLocalUnit;
    }

    private static Vector3 TransformExtents(Matrix4x4 matrix, Vector3 extents)
    {
        return new Vector3(
            Mathf.Abs(matrix.m00) * extents.x + Mathf.Abs(matrix.m01) * extents.y + Mathf.Abs(matrix.m02) * extents.z,
            Mathf.Abs(matrix.m10) * extents.x + Mathf.Abs(matrix.m11) * extents.y + Mathf.Abs(matrix.m12) * extents.z,
            Mathf.Abs(matrix.m20) * extents.x + Mathf.Abs(matrix.m21) * extents.y + Mathf.Abs(matrix.m22) * extents.z);
    }
}

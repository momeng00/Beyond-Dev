using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(Camera))]
public class ExpandCameraCullingArea : MonoBehaviour
{
    [Header("화면 밖으로 추가할 월드 좌표 범위")]
    [Min(0f)] public float horizontalPadding = 5f;
    [Min(0f)] public float verticalPadding = 5f;

    private Camera targetCamera;

    private void OnEnable()
    {
        targetCamera = GetComponent<Camera>();
        RenderPipelineManager.beginCameraRendering += ApplyExpandedCulling;
    }

    private void OnDisable()
    {
        RenderPipelineManager.beginCameraRendering -= ApplyExpandedCulling;

        if (targetCamera != null)
            targetCamera.ResetCullingMatrix();
    }

    private void ApplyExpandedCulling(
        ScriptableRenderContext context,
        Camera renderingCamera)
    {
        if (renderingCamera != targetCamera)
            return;

        if (!targetCamera.orthographic)
            return;

        float halfHeight =
            targetCamera.orthographicSize + verticalPadding;

        float halfWidth =
            targetCamera.orthographicSize * targetCamera.aspect
            + horizontalPadding;

        Matrix4x4 expandedProjection = Matrix4x4.Ortho(
            -halfWidth,
             halfWidth,
            -halfHeight,
             halfHeight,
            targetCamera.nearClipPlane,
            targetCamera.farClipPlane
        );

        targetCamera.cullingMatrix =
            expandedProjection * targetCamera.worldToCameraMatrix;
    }
}
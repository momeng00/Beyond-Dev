using UnityEngine;

public class FocusMaskController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera targetCamera;
    [SerializeField] private Material focusMaskMaterial;

    [Header("Default Values")]
    [SerializeField, Range(0f, 1f)] private float visibleAlpha = 0.9f;
    [SerializeField] private float defaultScale = 0.5f;

    private static readonly int OverlayColorId = Shader.PropertyToID("_OverlayColor");
    private static readonly int CenterId = Shader.PropertyToID("_Center");
    private static readonly int ScaleId = Shader.PropertyToID("_Scale");
    private static readonly int CutoffId = Shader.PropertyToID("_Cutoff");
    private static readonly int SoftnessId = Shader.PropertyToID("_Softness");

    private Color overlayColor;
    private Transform followTarget;
    private bool isVisible;

    private void Awake()
    {
        overlayColor = focusMaskMaterial.GetColor(OverlayColorId);
        HideImmediate();
        
        focusMaskMaterial.SetFloat(ScaleId, defaultScale);
        focusMaskMaterial.SetFloat(CutoffId, 0.1f);
        focusMaskMaterial.SetFloat(SoftnessId, 0.05f);
    }

    private void LateUpdate()
    {
        if (!isVisible || followTarget == null || targetCamera == null)
            return;

        SetCenterWorld(followTarget.position);
    }

    public void ShowAtViewport(Vector2 viewportPosition, float scale)
    {
        followTarget = null;
        isVisible = true;

        SetAlpha(visibleAlpha);
        SetCenterViewport(viewportPosition);
        SetScale(scale);
    }

    public void ShowAtWorld(Vector3 worldPosition, float scale)
    {
        followTarget = null;
        isVisible = true;

        SetAlpha(visibleAlpha);
        SetCenterWorld(worldPosition);
        SetScale(scale);
    }

    public void FollowTransform(Transform target, float scale)
    {
        followTarget = target;
        isVisible = true;

        SetAlpha(visibleAlpha);
        SetScale(scale);

        if (target != null)
            SetCenterWorld(target.position);
    }

    public void HideImmediate()
    {
        isVisible = false;
        followTarget = null;
        SetAlpha(0f);
    }

    public void SetCenterViewport(Vector2 viewportPosition)
    {
        focusMaskMaterial.SetVector(
            CenterId,
            new Vector4(viewportPosition.x, viewportPosition.y, 0f, 0f)
        );
    }

    public void SetCenterWorld(Vector3 worldPosition)
    {
        Vector3 viewport = targetCamera.WorldToViewportPoint(worldPosition);

        focusMaskMaterial.SetVector(
            CenterId,
            new Vector4(viewport.x, viewport.y, 0f, 0f)
        );
    }

    public void SetScale(float scale)
    {
        focusMaskMaterial.SetFloat(ScaleId, scale);
    }

    private void SetAlpha(float alpha)
    {
        overlayColor.a = alpha;
        focusMaskMaterial.SetColor(OverlayColorId, overlayColor);
    }
}
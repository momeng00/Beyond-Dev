using UnityEngine;

public class FocusMaskController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera targetCamera;
    [SerializeField] private Material focusMaskMaterial;

    [Header("Default Values")]
    [SerializeField, Range(0f, 1f)] private float visibleAlpha = 1.0f;
    private float defaultScale = 0.5f;
    public float DefaultScale
    {
        get { return defaultScale; }
    }

    private static readonly int OverlayColorId = Shader.PropertyToID("_OverlayColor");
    private static readonly int CenterId = Shader.PropertyToID("_Center");
    private static readonly int ScaleId = Shader.PropertyToID("_Scale");
    private static readonly int CutoffId = Shader.PropertyToID("_Cutoff");
    private static readonly int SoftnessId = Shader.PropertyToID("_Softness");
    private Color overlayColor;
    private bool isVisible;
    private static FocusMaskController activeController;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetActiveController()
    {
        activeController = null;
    }

    public static bool IsEffectActiveFor(Material material, Camera camera)
    {
        return activeController != null && activeController.isActiveAndEnabled &&
               activeController.isVisible && activeController.focusMaskMaterial == material &&
               activeController.targetCamera == camera;
    }

    private void Awake()
    {
        overlayColor = focusMaskMaterial.GetColor(OverlayColorId);
        InitFocusMaskMaterial();
        //focusMaskMaterial.SetFloat(ScaleId, defaultScale);
        //focusMaskMaterial.SetFloat(CutoffId, 0.1f);
        //focusMaskMaterial.SetFloat(SoftnessId, 0.05f);
    }

    public void InitFocusMaskMaterial()
    {
        isVisible = false;
        if (activeController == this)
            activeController = null;
        if (focusMaskMaterial == null)
            return;
        SetScale(10.0f);
        SetAlpha(0.0f);
    }

    private void OnDisable()
    {
        InitFocusMaskMaterial();
    }
    public float ReturnScale()
    {
        float startScale = focusMaskMaterial.GetFloat(ScaleId);
        return startScale;
    }
    public void ShowAtWorld(Vector3 worldPosition, float scale)
    {
        if (!isActiveAndEnabled || focusMaskMaterial == null || targetCamera == null)
            return;

        SetAlpha(visibleAlpha);
        SetCenterWorld(worldPosition);
        SetScale(scale);
        isVisible = true;
        activeController = this;
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

    public float ReturnAlpha()
    {
        return focusMaskMaterial.GetColor(OverlayColorId).a;
    }

    public void SetAlpha(float alpha)
    {
        overlayColor.a = alpha;
        focusMaskMaterial.SetColor(OverlayColorId, overlayColor);
    }


}

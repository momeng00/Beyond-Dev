using NUnit.Framework.Interfaces;
using System.Collections;
using UnityEngine;
using UnityEngine.Splines.Interpolators;

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
        SetScale(10.0f);
        SetAlpha(0.0f);
    }
    public float ReturnScale()
    {
        float startScale = focusMaskMaterial.GetFloat(ScaleId);
        return startScale;
    }
    public void ShowAtWorld(Vector3 worldPosition, float scale)
    {
        isVisible = true;

        SetAlpha(visibleAlpha);
        SetCenterWorld(worldPosition);
        SetScale(scale);
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

    public void SetAlpha(float alpha)
    {
        overlayColor.a = alpha;
        focusMaskMaterial.SetColor(OverlayColorId, overlayColor);
    }


}
using System.Reflection;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class MaterialControlTest : MonoBehaviour
{
    [Tooltip("FullScreenPassRendererFeature가 포함된 렌더러 데이터 에셋")]
    public ScriptableRendererData rendererData;
    // URP 에셋에서 렌더러 기능을 찾기 위한 변수들
    private Material targetMaterial;

    void Awake() 
    {
        // 1. 연결된 렌더러 데이터에서 FullScreenPassRendererFeature를 찾습니다.
        foreach (var feature in rendererData.rendererFeatures)
        {
            if (feature is FullScreenPassRendererFeature)
            {
                // 2. 기능에서 Material을 직접 가져옵니다.
                targetMaterial = (feature as FullScreenPassRendererFeature).passMaterial;
                break;
            }
        }

        if (targetMaterial == null)
        {
            Debug.LogError("해당 렌더러 데이터에서 FullScreenPassRendererFeature를 찾을 수 없습니다.");
        }
    }

    //SetMaterialFloat("_Blur_Offset", blurValue);
    public void SetMaterialFloat(string propertyName, float value)
    {
        if (targetMaterial != null)
        {
            targetMaterial.SetFloat(propertyName, value);
        }
    }


    private void OnApplicationQuit()
    {
        SetMaterialFloat("_Blur_Offset", 0);
    }
}
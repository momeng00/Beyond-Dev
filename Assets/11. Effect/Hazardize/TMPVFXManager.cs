using TMPro;
using UnityEngine;
using UnityEngine.VFX;

[System.Serializable]
public class TMPVFXPair
{
    public TextMeshPro tmp;
    public VisualEffect vfx;

    [HideInInspector] public Mesh tmpMesh;
}

public class TMPVFXManager : MonoBehaviour
{
    public TMPVFXPair[] pairs;

    private void Update()
    {
        if (Input.anyKeyDown)
        {
            InitTMP();
        }
    }
    public void InitTMP()
    {
        foreach (var p in pairs)
        {
            if (p.tmp == null || p.vfx == null) continue;

            // 1) TMP Mesh 추출
            p.tmp.ForceMeshUpdate();
            p.tmpMesh = p.tmp.mesh;

            // 2) TMP SDF 아틀라스 텍스처 추출
            Texture sdfTex = p.tmp.fontMaterial.GetTexture("_MainTex");

            // 3) TMP 머테리얼 Color 추출
            // TMP 기본 SDF 셰이더는 "_FaceColor" 프로퍼티 사용
            Color faceColor = Color.white;
            if (p.tmp.fontMaterial.HasProperty("_Color"))
                faceColor = p.tmp.fontMaterial.GetColor("_Color");

            // 4) VFX에 전달
            if (p.vfx.HasMesh("SourceMesh"))
                p.vfx.SetMesh("SourceMesh", p.tmpMesh);

            if (p.vfx.HasTexture("TextSDF"))
                p.vfx.SetTexture("TextSDF", sdfTex);

            if (p.vfx.HasFloat("SDF_Threshold"))
                p.vfx.SetFloat("SDF_Threshold", 0.5f);

            if (p.vfx.HasVector4("Color")) // VFX Graph에서는 보통 Color를 Vector4로 받음
                p.vfx.SetVector4("Color", faceColor);
        }
    }
}

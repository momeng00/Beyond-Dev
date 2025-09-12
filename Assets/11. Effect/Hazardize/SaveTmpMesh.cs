using UnityEngine;
using UnityEditor;
using TMPro;

public class SaveTmpMesh : MonoBehaviour
{
    [ContextMenu("Save TMP Mesh As Asset")]
    void SaveTmpMeshAsset()
    {
        TextMeshPro tmp = GetComponent<TextMeshPro>();
        if (tmp == null)
        {
            Debug.LogError("이 컴포넌트는 TextMeshPro (3D) 오브젝트에 붙여야 합니다.");
            return;
        }

        // TMP가 쓰는 메쉬 강제 업데이트
        tmp.ForceMeshUpdate();
        Mesh mesh = Instantiate(tmp.mesh); // 복제해야 에셋 저장 가능

        // 저장
        string path = "Assets/TmpMesh.asset";
        AssetDatabase.CreateAsset(mesh, path);
        AssetDatabase.SaveAssets();

        Debug.Log($"TMP Mesh 저장 완료: {path}");
    }
}

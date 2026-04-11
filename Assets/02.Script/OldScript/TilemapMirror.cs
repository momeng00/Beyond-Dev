using UnityEngine;
using UnityEngine.Tilemaps;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class TilemapMirror : MonoBehaviour
{
    [Header("Source / Target")]
    public Tilemap sourceTilemap;   // A 타일맵
    public Tilemap targetTilemap;   // B 타일맵

    [Header("Options")]
    public bool clearTargetBeforeCopy = true;
    public bool copyColor = true;
    public bool copyTransform = true;

    public void CopyAllTiles()
    {
        if (sourceTilemap == null || targetTilemap == null)
        {
            Debug.LogWarning("Source 또는 Target Tilemap이 비어 있습니다.", this);
            return;
        }

        if (sourceTilemap == targetTilemap)
        {
            Debug.LogWarning("Source와 Target Tilemap이 같을 수 없습니다.", this);
            return;
        }

        BoundsInt bounds = sourceTilemap.cellBounds;

        if (clearTargetBeforeCopy)
        {
            targetTilemap.ClearAllTiles();
        }

        foreach (Vector3Int pos in bounds.allPositionsWithin)
        {
            TileBase tile = sourceTilemap.GetTile(pos);

            if (tile == null)
                continue;

            targetTilemap.SetTile(pos, tile);

            if (copyTransform)
                targetTilemap.SetTransformMatrix(
                    pos,
                    sourceTilemap.GetTransformMatrix(pos)
                );

            if (copyColor)
                targetTilemap.SetColor(
                    pos,
                    sourceTilemap.GetColor(pos)
                );
        }

        targetTilemap.RefreshAllTiles();

#if UNITY_EDITOR
        EditorUtility.SetDirty(targetTilemap);
#endif

        Debug.Log("Tilemap copy complete.", this);
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(TilemapMirror))]
public class TilemapMirrorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        GUILayout.Space(10);

        TilemapMirror mirror = (TilemapMirror)target;

        if (GUILayout.Button("Update Tilemap"))
        {
            Undo.RegisterCompleteObjectUndo(
                mirror.targetTilemap,
                "Update Tilemap Mirror"
            );

            mirror.CopyAllTiles();
        }
    }
}
#endif
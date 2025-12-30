using UnityEngine;
using UnityEditor;

public class SortOrderInLayerByHierarchy
{
    [MenuItem("GameObject/정리/자식 Order In Layer 정렬", false, 0)]
    static void SortOrder()
    {
        GameObject parent = Selection.activeGameObject;

        if (parent == null)
        {
            return;
        }

        int order = 0;

        // 하이어라키 순서 그대로 (위 → 아래)
        for (int i = 0; i < parent.transform.childCount; i++)
        {
            Transform child = parent.transform.GetChild(i);

            SpriteRenderer sr = child.GetComponent<SpriteRenderer>();
            if (sr == null)
                continue;

            Undo.RecordObject(sr, "Sort Order In Layer");

            sr.sortingOrder = order;
            order++;
        }

    }
}

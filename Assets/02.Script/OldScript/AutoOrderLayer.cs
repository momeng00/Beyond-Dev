using UnityEngine;

[ExecuteAlways]
public class SortingOrderFollower : MonoBehaviour
{
    private SpriteRenderer topParentSpriteRenderer;
    private Renderer selfRenderer;

    private void OnEnable()
    {
        CacheComponents();
        UpdateSortingOrder_EditorOnly();
    }

    private void OnValidate()
    {
        CacheComponents();
        UpdateSortingOrder_EditorOnly();
    }

    private void Update()
    {
#if UNITY_EDITOR
        // 에디터에서만 실시간 업데이트
        if (!Application.isPlaying)
            UpdateSortingOrder_EditorOnly();
#endif
    }

    private void CacheComponents()
    {
        if (selfRenderer == null)
            selfRenderer = GetComponent<Renderer>();

        topParentSpriteRenderer = FindTopParentSpriteRenderer();
    }

    private SpriteRenderer FindTopParentSpriteRenderer()
    {
        Transform current = transform.parent;
        SpriteRenderer lastFound = null;

        while (current != null)
        {
            var sr = current.GetComponent<SpriteRenderer>();
            if (sr != null)
                lastFound = sr;

            current = current.parent;
        }

        return lastFound;
    }

    /// <summary>
    /// 에디터에서만 정렬값을 부모 기반으로 자동 설정
    /// </summary>
    private void UpdateSortingOrder_EditorOnly()
    {
        if (Application.isPlaying) return; // 실행 중에는 고정 상태 유지

        if (selfRenderer == null || topParentSpriteRenderer == null)
            return;

        selfRenderer.sortingOrder = topParentSpriteRenderer.sortingOrder + 1;
    }
}

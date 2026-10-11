using UnityEngine;

public class BuildingBlock : MonoBehaviour
{
    [SerializeField] private SpriteRenderer connectionImage;
    [SerializeField] private Transform entranceStopPoint;

    public bool HasEntranceStopPoint
    {
        get
        {
            if (entranceStopPoint == null) return false;
            if (entranceStopPoint == transform) return true;
            return entranceStopPoint.IsChildOf(transform);
        }
    }
    public Vector3 EntranceStopPosition => entranceStopPoint.position;

    // Only this renderer determines the connection points, never child renderers.
    public bool HasValidImage
    {
        get
        {
            if (connectionImage == null)
                return false;

            if (connectionImage.sprite == null)
                return false;

            Transform imageTransform = connectionImage.transform;
            if (imageTransform != transform &&
                !imageTransform.IsChildOf(transform))
                return false;

            return connectionImage.bounds.size.y > 0.001f;
        }
    }

    public Vector3 TopCenter
    {
        get
        {
            Bounds bounds = connectionImage.bounds;
            return new Vector3(bounds.center.x, bounds.max.y, bounds.center.z);
        }
    }

    public Vector3 BottomCenter
    {
        get
        {
            Bounds bounds = connectionImage.bounds;
            return new Vector3(bounds.center.x, bounds.min.y, bounds.center.z);
        }
    }

    public void AlignTopTo(Vector3 position)
    {
        transform.position += position - TopCenter;
    }
}

using UnityEngine;

public abstract class Platform : MonoBehaviour
{
    [SerializeField]protected LayerMask layerMask;
    protected Collider2D col;

    virtual protected bool IsInLayerMask(GameObject obj, LayerMask mask)
    {
        return ((1 << obj.layer) & mask) != 0;
    }
}

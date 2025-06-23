using UnityEngine;

public abstract class Spot : MonoBehaviour, IInteract
{
    [SerializeField] protected LayerMask layerMask;
    virtual public void Interact()
    {
        
    }
    virtual protected bool IsInLayerMask(GameObject obj, LayerMask mask)
    {
        return ((1 << obj.layer) & mask) != 0;
    }
}

using UnityEngine;

public abstract class Switch : MonoBehaviour, IInteract
{
    [SerializeField] protected LayerMask layerMask;
    public virtual void Interact()
    {

    }
    virtual protected bool IsInLayerMask(GameObject obj, LayerMask mask)
    {
        return ((1 << obj.layer) & mask) != 0;
    }
    virtual public void SetSwitch(ISwitchable node)
    {

    }
}

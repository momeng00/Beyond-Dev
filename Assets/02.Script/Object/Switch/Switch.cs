using UnityEngine;

public abstract class Switch : MonoBehaviour, IInteract
{
    [SerializeField] protected LayerMask layerMask;
    protected Animator ani;
    virtual public void Awake()
    {
        ani = GetComponent<Animator>();
    }
    public virtual void Interact()
    {

    }

    virtual protected void IsDetected(bool activate)
    {
        ani.SetBool("activate", activate );
    }
    virtual protected bool IsInLayerMask(GameObject obj, LayerMask mask)
    {
        return ((1 << obj.layer) & mask) != 0;
    }
    virtual public void SetSwitch(ISwitchable node)
    {

    }
}

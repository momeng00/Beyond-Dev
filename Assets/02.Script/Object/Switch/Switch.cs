using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class Switch : MonoBehaviour, IInteract
{
    [SerializeField] protected LayerMask layerMask;
    protected Animator ani;
    protected Material materialInstance;
    protected Renderer myRenderer;
    public Action<bool> OnSwitchAction;
    virtual public void Awake()
    {
        ani = GetComponent<Animator>();
        myRenderer = GetComponent<Renderer>();
        materialInstance = myRenderer.material;
    }
    public virtual void Interact()
    {
        
    }
    protected void ApplyToTargets(IEnumerable<ISwitchable> targets, bool state, Action<bool> onResult = null)
    {
        foreach (var target in targets)
        {
            bool accepted = target.SwitchOn(state);
            onResult?.Invoke(accepted);
        }
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

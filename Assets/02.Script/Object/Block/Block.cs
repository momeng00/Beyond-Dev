using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Block : MonoBehaviour, IReset
{
    protected Collider2D col;
    protected Rigidbody2D rb;
    public Action<bool> blockEvent;
    protected SpriteMask mask;
    private List<IEventListener> eventListeners;
    public float toggleDelay;
    private Coroutine activateCoroutine;
    virtual public bool BlockState { get; set; }

    public virtual void Start()
    {
        mask = GetComponent<SpriteMask>();
        col = GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();
        GameManager.Instance.RegisterInitAction(ResetAction);
        GameManager.Instance.OnReset += ResetAction;
    }

    public virtual void OnBlockAction()
    {

    }
    public virtual void InitializeReset()
    {

    }

    public virtual void ResetAction()
    {

    }

    protected void ToggleEventChildren()
    {
        IEventListener[] listeners = GetComponentsInChildren<IEventListener>();
        eventListeners = new List<IEventListener>(listeners);
        eventListeners.Sort((a, b) => a.ToggleEventPriority.CompareTo(b.ToggleEventPriority));
    }
    public void RunToggleEvent(bool state)
    {
        if (state)
        {
            activateCoroutine = StartCoroutine(RunToggleEventDelay(state));
        }
        else
        {
            foreach (var listener in eventListeners)
            {
                listener.ToggleEvent(BlockState);
            }
            if (activateCoroutine == null)
                return;
            StopCoroutine(activateCoroutine);
        }
    }

    IEnumerator RunToggleEventDelay(bool state)
    {
        foreach (var listener in eventListeners)
        {
            listener.ToggleEvent(BlockState);
            yield return new WaitForSeconds(toggleDelay);
        }
    }
}

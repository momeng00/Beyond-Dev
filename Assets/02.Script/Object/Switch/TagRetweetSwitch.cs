using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

public class TagRetweetSwitch : Switch, IReset  
{
    public Renderer tagRenderer;
    public Renderer retweetRenderer;
    private Material tagMaterial;
    private Material retweetMaterial;
    public MovingwalkDirection targetDirection;
    public event Action<MovingwalkDirection> OnDirection;
    private BoxCollider2D col;
    public bool SwitchState
    {
        get
        {
            return _switchState;
        }
        set
        {
            _switchState = value;
            OnSwitchAction?.Invoke(value);
        }
    }
    private bool _switchState;
    private List<ISwitchable> targetBlock = new List<ISwitchable>();
    private bool isSatisfied;
    new private void Awake()
    {
        col = GetComponent<BoxCollider2D>();
        tagMaterial = tagRenderer.material;
        retweetMaterial = retweetRenderer.material;
    }

    private void Start()
    {
        col.isTrigger = true;
        SwitchState = false;
        InputSystem.Instance.RegisterAction(KeyState.Play_Key, KeyCode.E, Interact);
        GameManager.Instance.OnReset += ResetAction;
    }
    public override void SetSwitch(ISwitchable node)
    {
        base.SetSwitch(node);
        RetweetBlock functionBlockTarget = node as RetweetBlock;
        if (functionBlockTarget != null)
        {
            OnDirection += functionBlockTarget.SetMovingwalkDirection;
        }
        targetBlock.Add(node);
    }
    protected override void IsDetected(bool activate)
    {
        tagRenderer.gameObject.GetComponent<Animator>().SetBool("activate", activate);
        retweetRenderer.gameObject.GetComponent<Animator>().SetBool("activate", activate);
    }
    public override void Interact()
    {
        base.Interact();
        if (isSatisfied)
        {
            SwitchState = !SwitchState;
            IsDetected(SwitchState);
            OnDirection?.Invoke(targetDirection);
            foreach (ISwitchable switchable in targetBlock)
            {
                switchable.SwitchOn(SwitchState);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (IsInLayerMask(collision.gameObject, layerMask))
        {
            isSatisfied = true;
            tagMaterial.SetFloat("_IsHovered", 1.0f);
            retweetMaterial.SetFloat("_IsHovered", 1.0f);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (IsInLayerMask(collision.gameObject, layerMask))
        {
            isSatisfied = false;
            tagMaterial.SetFloat("_IsHovered", 0.0f);
            retweetMaterial.SetFloat("_IsHovered", 0.0f);
        }
    }

    public void ResetAction()
    {
        isSatisfied = false;
        tagMaterial.SetFloat("_IsHovered", 0.0f);
        retweetMaterial.SetFloat("_IsHovered", 0.0f);
    }

    public void InitializeReset()
    {
        throw new NotImplementedException();
    }
}
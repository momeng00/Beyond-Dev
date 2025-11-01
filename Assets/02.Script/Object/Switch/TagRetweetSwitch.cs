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
    private Collider2D col;
    public bool SwitchState
    {
        get
        {
            return _switchState;
        }
        set
        {
            _switchState = value;
        }
    }
    private bool _switchState;
    private List<ISwitchable> targetBlock = new List<ISwitchable>();
    private bool isSatisfied;
    new private void Awake()
    {
        col = GetComponent<Collider2D>();
        tagMaterial = tagRenderer.material;
        retweetMaterial = retweetRenderer.material;
    }
    private void Start()
    {
        col.isTrigger = true;
        InputSystem.Instance.RegisterAction(KeyState.Play_Key, KeyCode.E, Interact);
    }
    public override void SetSwitch(ISwitchable node)
    {
        base.SetSwitch(node);
        FunctionBlock functionBlockTarget = node as FunctionBlock;
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

    public void InitializeReset()
    {
        throw new System.NotImplementedException();
    }

    public void ResetAction()
    {
        throw new System.NotImplementedException();
    }
}
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class RetweetSwitch : Switch, IReset
{
    private List<Rigidbody2D> movingLists = new List<Rigidbody2D>();
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
    private BoxCollider2D _collider;
    private bool _switchState;
    public MovingwalkDirection targetDirection;
    public List<ISwitchable> targetBlock = new List<ISwitchable>();
    public event Action<MovingwalkDirection> OnDirection;
    private bool isSatisfied;
    
    public override void Awake()
    {
        base.Awake();
        _collider = GetComponent<BoxCollider2D>();
    }
    private void Start()
    {
        _collider.isTrigger = true;
        SwitchState = false;
        InputSystem.Instance.RegisterAction(KeyState.Play_Key, KeyCode.E, Interact);
        GameManager.Instance.OnReset += ResetAction;
    }
    protected override void IsDetected(bool activate)
    {
        base.IsDetected(activate);
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
            materialInstance.SetFloat("_IsHovered", 1.0f);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (IsInLayerMask(collision.gameObject, layerMask))
        {
            isSatisfied = false;
            materialInstance.SetFloat("_IsHovered", 0.0f);
        }
    }

    public void ResetAction()
    {
        isSatisfied = false;
        materialInstance.SetFloat("_IsHovered", 0.0f);
        myRenderer.material = materialInstance;
    }

    protected override bool IsInLayerMask(GameObject obj, LayerMask mask)
    {
        return base.IsInLayerMask(obj, mask);
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

    public void InitializeReset()
    {
        throw new NotImplementedException();
    }
}
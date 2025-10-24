using System;
using System.Collections.Generic;
using UnityEngine;

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
    [SerializeField]private bool isSatisfied;
    private void Start()
    {
        _collider = GetComponent<BoxCollider2D>();
        _collider.isTrigger = true;
        SwitchState = false;
        InputSystem.Instance.RegisterAction(KeyState.Play_Key, KeyCode.E, Interact);
        GameManager.Instance.OnReset += ResetAction;
    }
    public override void Interact()
    {
        base.Interact();
        if (isSatisfied)
        {
            SwitchState = !SwitchState;
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
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (IsInLayerMask(collision.gameObject, layerMask))
        {
            isSatisfied = false;
        }
    }

    public void ResetAction()
    {
        isSatisfied = false;
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
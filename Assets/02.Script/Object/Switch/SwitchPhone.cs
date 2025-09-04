using System.Collections.Generic;
using UnityEngine;

public class SwitchPhone : Switch,IReset
{
    public List<ISwitchable> targetBlock = new List<ISwitchable>();
    [SerializeField]private bool isSatisfied;
    private bool _switchState;
    private Collider2D col;

    private void Start()
    {
        this.gameObject.AddComponent<BoxCollider2D>();
        col = GetComponent<Collider2D>();
        col.isTrigger = true;
        _switchState = false;
        GameManager.Instance.OnReset += ResetAction;
        InputSystem.Instance.RegisterAction(KeyState.Play_Key,KeyCode.E,Interact);
    }
    public void InitializeReset()
    {
        
    }
    public override void SetSwitch(ISwitchable node)
    {
        base.SetSwitch(node);
        targetBlock.Add(node);
    }

    public override void Interact()
    {
        base.Interact();
        if (isSatisfied)
        {
            _switchState = !_switchState;
            foreach (var block in targetBlock)
            {
                block.SwitchOn(_switchState);
            }
        }
    }

    public void ResetAction()
    {
       isSatisfied = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (IsInLayerMask(collision.gameObject,layerMask))
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
}
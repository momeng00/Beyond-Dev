using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockSwitch : Switch,IReset
{

    public List<ISwitchable> targetBlock = new List<ISwitchable>();
    private bool isSatisfied;
    [Header("카메라 스위치일때")]
    public bool isCamera = false;

    public bool IsSatisfied
    {
        get { return isSatisfied; }
    }
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
    private Collider2D col;

    public override void Awake()
    {
        base.Awake();
        col = GetComponent<Collider2D>();
        col.isTrigger = true;
    }
    private void Start()
    {
        SwitchState = false;
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
        if (isSatisfied)
        {
            if (isCamera)
            {
                AudioManager.Instance.PlayOneShotSFXAudio(AudioName.CameraSwitch);
            }

            AudioManager.Instance.PlaySFXAudio(AudioName.Switch);
            SwitchState = !SwitchState;
            foreach (var block in targetBlock)
            {
                if(block.SwitchOn(SwitchState))
                    IsDetected(SwitchState);
            }
        }
    }

    public void ResetAction()
    {
        isSatisfied = false;
        SwitchState = false;
        ani.SetBool("activate", false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (IsInLayerMask(collision.gameObject,layerMask))
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
}
using System.Collections.Generic;
using UnityEngine;

public class DownloadStationSwtich : Switch, IReset
{
    public List<ISwitchable> target = new List<ISwitchable>();
    private bool isEnter;
    [HideInInspector]public bool isUpload;
    [HideInInspector]public new Animator ani;

    public bool IsSatisfied
    {
        get { return isEnter && isUpload; }
    }

    private Collider2D col;

    public override void Awake()
    {
        base.Awake();
        ani = GetComponent<Animator>();
        col = GetComponent<Collider2D>();
        col.isTrigger = true;
    }
    private void Start()
    {
        GameManager.Instance.OnReset += ResetAction;
        InputSystem.Instance.RegisterAction(KeyState.Play_Key, KeyCode.E, Interact);
    }

    public override void SetSwitch(ISwitchable node)
    {
        base.SetSwitch(node);
        target.Add(node);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (IsInLayerMask(collision.gameObject, layerMask))
        {
            isEnter = true;
            materialInstance.SetFloat("_IsHovered", 1.0f);
        }
    }
    public override void Interact()
    {
        base.Interact();
        if (IsSatisfied)
        {
            foreach (var block in target)
            {
                block.SwitchOn(true);
            }
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (IsInLayerMask(collision.gameObject, layerMask))
        {
            isEnter = false;
            materialInstance.SetFloat("_IsHovered", 0.0f);
        }
    }

    public void ResetAction()
    {
        isEnter = false;
    }

    public void InitializeReset()
    { 

    }
}
using UnityEngine;

public class SingleMessageBlock : Block, ISwitchable
{
    Vector2 startPos;
    private bool _startState;
    [SerializeField]private bool _blockState;
    public bool blockState
    {
        get
        {
            return _blockState;
        }
        set
        {
            _blockState = value;
            MessagerBlock(value);
        }
    }

    public Switch Switch
    {
        get
        {
            return _Switch;
        }
        set
        {
            _Switch = value;
            MessagerBlock(value);
        }
    }
    [SerializeField]Switch _Switch;

    public override void Start()
    {
        base.Start();
        InitializeReset();
        GameManager.Instance.RegisterInitAction(ResetAction);
        GameManager.Instance.OnReset += ResetAction;
        Switch.SetSwitch(this);
        MessagerBlock(blockState);
    }
    

    public void MessagerBlock(bool on)
    {
        //ani.SetBool("BlockState",blockState);
        gameObject.SetActive(on);
    }

    public void SwitchOn(bool value)
    {
        blockState = !blockState;
    }
    public override void InitializeReset()
    {
        base.InitializeReset();
        startPos = transform.position;
        _startState= blockState;
    }
    public override void ResetAction()
    {
        base.ResetAction();
        transform.position = startPos;
        blockState = _startState;
    }
    public override void OnBlockAction()
    {
        base.OnBlockAction();
        _blockState = !_blockState;
    }

}
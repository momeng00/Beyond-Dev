using UnityEngine;

public class SingleMessageBlock : Block, ISwitchable
{
    private Collider2D col;
    private Animator ani;
    private bool _blockState;
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
    private void Start()
    {
        _blockState = false;
        col = GetComponent<Collider2D>();
        ani = GetComponent<Animator>();
    }
    public override void OnBlockAction()
    {
        base.OnBlockAction();
        _blockState = !_blockState;
    }

    public void MessagerBlock(bool on)
    {
        ani.SetBool("BlockState",blockState);
    }

    public void SwitchOn()
    {
        throw new System.NotImplementedException();
    }

    public void SwitchOff()
    {
        throw new System.NotImplementedException();
    }
}
using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
public enum MovingwalkDirection
{
    Stop = 0,  
    Left = -1,  
    Right = 1  
}
public class FunctionBlock : Block, ISwitchable
{
    private Animator ani;
    public MovingwalkDirection movingWalkDirection;
    // 1. Swtich가 타입캐스팅을 통해서 해결.
    // 2. Block이 상속받는 곳에 메서드를 추가하는 방식으로 해결.
    // 3. Struct를 통해서 switchData를 제작. (Object를 통해서 해결)
    //이벤트를 통해서 전달하는 방식으로 할 수 있을까?
    //타입캐스팅을 등록할 때 하는 방식으로 가능할 것 같은데.

    //처음에 스위치룰 둥록할때 타입케이스을 통해서 이벤트를 등록해야함. 
    //누가 이벤트를 가지고있어야 하나?
    public float speed;

    public List<Switch> switchList = new List<Switch>();
    private List<IMovable> movingTargets = new List<IMovable>();
    
    public bool BlockState
    {
        get 
        { 
            return _blockState; 
        }
        set
        {
            _blockState = value;
            if (_blockState)
            {
                foreach (var moving in movingTargets)
                {
                    moving.AddExtraVelocity(this, new Vector2(speed * (int)movingWalkDirection, 0f));
                }
            }
            else
            {
                movingWalkDirection = MovingwalkDirection.Stop;
                foreach (var moving in movingTargets)
                {
                    moving.RemoveExtraVelocity(this);
                }
            }
            ani.SetInteger("Direction",(int)movingWalkDirection);
        }
    }

    public Switch Switch => throw new NotImplementedException();

    private bool _blockState;

    private void Awake()
    {
        ani = GetComponent<Animator>();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void Start()
    {
        base.Start();
        foreach (Switch sw in switchList)
        {
            sw.SetSwitch(this);
        }
        
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        IMovable otherRb = collision.gameObject.GetComponent<IMovable>(); ;
        if (otherRb != null && !movingTargets.Contains(otherRb))
        {
            movingTargets.Add(otherRb);
        }
        if (_blockState)
        {
            foreach (IMovable moving in movingTargets)
            {
                moving.AddExtraVelocity(this, new Vector2(speed * (int)movingWalkDirection, 0f));
            }
        }
    }


    private void OnCollisionExit2D(Collision2D collision)
    {
        IMovable otherRb = collision.gameObject.GetComponent<IMovable>(); ;
        if (otherRb != null)
        {
            movingTargets.Remove(otherRb);
            otherRb.RemoveExtraVelocity(this);
        }
    }

    public void SwitchOn(bool value)
    {
        BlockState = !BlockState;
    }

    public void SetMovingwalkDirection(MovingwalkDirection direction)
    {
        movingWalkDirection = direction;
    }
}

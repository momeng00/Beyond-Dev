using System;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
public enum MovingwalkDirection
{
    Stop = 0,  
    Left = -1,  
    Right = 1
}
public class RetweetBlock : Block, ISwitchable
{
    private Animator ani;
    [HideInInspector] public MovingwalkDirection movingWalkDirection;
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
    private Material materialInstance;
    private Renderer myRenderer;
    private BoxCollider2D BoxCollider2D;
    override public bool BlockState
    {
        get
        {
            return _blockState;
        }
        set
        {
            _blockState = value;
            mask.enabled = value;
            blockEvent?.Invoke(value);
            RunToggleEvent(value);
            if (_blockState)
            {
                matarialAnim.Play();
                foreach (var moving in movingTargets)
                {
                    moving.AddExtraVelocity(this, new Vector2(speed * (int)movingWalkDirection, 0f));
                }
            }
            else
            {
                matarialAnim.PlayReturn();
                movingWalkDirection = MovingwalkDirection.Stop;
                foreach (var moving in movingTargets)
                {
                    moving.RemoveExtraVelocity(this);
                }
            }
            ani.SetInteger("Direction", (int)movingWalkDirection);
        }
    }

    public Switch Switch => throw new NotImplementedException();

    private bool _blockState;

    private void Awake()
    {
        ani = GetComponent<Animator>();
        myRenderer = GetComponent<Renderer>();
        materialInstance = myRenderer.material;
        ToggleEventChildren();
        BoxCollider2D = GetComponent<BoxCollider2D>();
        BoxCollider2D.size = myRenderer.bounds.size;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void Start()
    {
        base.Start();
        foreach (Switch sw in switchList)
        {
            sw.SetSwitch(this);
        }
        materialInstance.SetVector("_SpriteSize", myRenderer.bounds.size);
        BlockState = false;
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        ////1.부딪힌 오브젝트의 경계(bounds)를 가져옵니다.
        //bounds otherbounds = collision.collider.bounds;

        ////2.이 오브젝트(센서)의 경계를 가져옵니다.
        //if (mycollider == null)
        //{
        //    mycollider = getcomponent<collider2d>();
        //}
        //bounds mybounds = mycollider.bounds;


        ////3.상대방의 '발끝'(가장 낮은 y값)이 나의 '머리끝'(가장 높은 y값)보다 위에 있거나 같은지 확인합니다.
        //float otherbottomedge = otherbounds.center.y - otherbounds.extents.y;
        //float mytopedge = mybounds.center.y + mybounds.extents.y;
        //debug.log(otherbottomedge >= mytopedge);
        //if (isinlayermask(collision.gameobject, detectedlayer) && otherbottomedge >= (mytopedge - 0.05f))
        //{
        //    debug.log("위로 올라감 진입");
        //    controller.onobjectentered(groupname, collision.gameobject);
        //}
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

    public bool SwitchOn(bool value)
    {
        BlockState = !BlockState;
        return true;
    }

    public void SetMovingwalkDirection(MovingwalkDirection direction)
    {
        movingWalkDirection = direction;
    }

    public override void ResetAction()
    {
        base.ResetAction();
        BlockState = false;
    }


     
}

using System.Collections.Generic;
using System.Xml;
using TMPro;
using UnityEngine;

public class SingleMessageBlock : Block, ISwitchable
{
    //스위치를 위한 블럭으로 변경예정
    public LocalizationKeys localizationKeys;
    TextMeshPro tmpText;
    Vector2 startPos;
    private bool _startState;
    [SerializeField]private bool _blockState;

    private BoxCollider2D boxCollider;
    private SpriteRenderer spriteRenderer;
    public LayerMask targetLayer;
    private List<Rigidbody2D> rigidbodiesInTrigger = new List<Rigidbody2D>();
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
    [SerializeField]private Switch _Switch;

    public override void Start()
    {
        base.Start();
        InitializeReset();
        GameManager.Instance.RegisterInitAction(ResetAction);
        GameManager.Instance.OnReset += ResetAction;
        Switch.SetSwitch(this);
        MessagerBlock(blockState);
    }
    private void Awake()
    {
        tmpText = GetComponent<TextMeshPro>();
        boxCollider = GetComponent<BoxCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 지정된 레이어인지 확인하고 Rigidbody2D가 있는지 확인
        if (((1 << collision.gameObject.layer) & targetLayer) != 0)
        {
            Rigidbody2D rb = collision.GetComponent<Rigidbody2D>();
            if (rb != null && !rigidbodiesInTrigger.Contains(rb))
            {
                rigidbodiesInTrigger.Add(rb);
            }
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (((1 << collision.gameObject.layer) & targetLayer) != 0)
        {
            Rigidbody2D rb = collision.GetComponent<Rigidbody2D>();
            if (rb != null && rigidbodiesInTrigger.Contains(rb))
            {
                // 안에서 '오브젝트가 SetActive(false)가 되면 오류 확인을 해야합니다'
                rigidbodiesInTrigger.Remove(rb);
            }
        }
    }

    public void MessagerBlock(bool on)
    {
        //ani.SetBool("BlockState",blockState);
        if (on) // 블록이 켜질 때
        {
            // 리스트에 있는 모든 오브젝트를 위로 올림
            MoveObjectsUp();
            if(tmpText != null)
            {
                tmpText.enabled = true;
            }
            spriteRenderer.enabled = true; // 보이게
            boxCollider.isTrigger = false; // 단단한 발판으로
        }
        else // 블록이 꺼질 때
        {
            if (tmpText != null)
            {
                tmpText.enabled = false;
            }
            spriteRenderer.enabled = false; // 안 보이게
            boxCollider.isTrigger = true; // 감지 모드로
        }
    }
    private void MoveObjectsUp()
    {
        // 리스트를 복사해서 순회 (원본 리스트가 변경될 수 있으므로)
        List<Rigidbody2D> rigidbodiesToMove = new List<Rigidbody2D>(rigidbodiesInTrigger);

        foreach (Rigidbody2D rb in rigidbodiesToMove)
        {
            if (rb == null) continue; // 오브젝트가 파괴되었을 경우 대비

            float objectHeight = rb.GetComponent<Collider2D>().bounds.size.y;
            float boxTopY = transform.position.y + boxCollider.offset.y + (boxCollider.size.y / 2);

            Vector2 targetPosition = new Vector2(
                rb.position.x,
                boxTopY + (objectHeight / 2)
            );

            rb.position = targetPosition; // 즉시 위치 변경
        }
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
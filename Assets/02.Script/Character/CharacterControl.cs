using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.UI.Image;

public class CharacterControl : MonoBehaviour, IReset
{
    #region 컴포넌트 선언부
    private Rigidbody2D _rb;
    private Animator _ani;
    #endregion


    private float _axisX; //이동되는 수치 받을 힘
    private float _axisY; //이동되는 수치 받을 힘
    private float _direction =1f;
    public float direction
    {
        set
        {
            if(_direction != value)
            {
                _direction = value;
                if (value > 0f) 
                {
                    transform.localScale = new Vector3(1f, 1f, 1f);
                }
                else if(value < 0f)
                {
                    transform.localScale = new Vector3(-1f, 1f, 1f);
                }
                
            }
        }
    }

    [SerializeField]private Vector2 startPos;
    public Vector2 handOffset;
    public float handDistance;
    [SerializeField] private LayerMask handLayerMask;

    public bool isHandFull
    {
        get
        {
            Collider2D handObj = null;
            RaycastHit2D hit = Physics2D.Raycast((Vector2)transform.position + handOffset,
                                                    new Vector2(_direction,0f),
                                                    handDistance,
                                                    handLayerMask);
            Debug.DrawRay((Vector2)transform.position + handOffset, new Vector2(_direction, 0f) * handDistance, hit.collider ? Color.red : Color.green);
            handObj = hit.collider;
            return handObj;
        }
    }
    public bool isGrounded
    {
        get
        {
            _col = Physics2D.OverlapBox((Vector2)transform.position + footOffset, 
                new Vector2(transform.localScale.x, 0.2f),
                0.0f,
                groundMask);
            return _col;
        }
    }

    private bool _canMove;
    private AerialState _aerialState;

    public float moveSpeed;
    public float jumpForce;
    public Vector2 footOffset;

    private List<IInteract> interacts = new List<IInteract>();
    
    public LayerMask groundMask;

    private Collider2D _col;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Initialize();
    }
    private void Initialize()
    {
        InputSystem.Instance.RegisterAction(KeyState.Play_Key, "Horizontal", Move);
        InputSystem.Instance.RegisterAction(KeyState.Play_Key, KeyCode.Space, Jump);
        GameManager.Instance.RegisterInitAction(InitializeReset);
        GameManager.Instance.OnReset += ResetAction;
        InitializeReset();
    }
    private void Awake()
    {
        _canMove = true;
        _ani = GetComponent<Animator>();
        _rb = GetComponent<Rigidbody2D>();
        _axisY = 0;
        _axisX = 0;
    }
    // Update is called once per frame
    void Update()
    {

    }
    private void FixedUpdate()
    {
        _rb.linearVelocity = new Vector2(_axisX * moveSpeed , _rb.linearVelocityY);
    }
    private void Move(float horizontal)
    {
        if(!_canMove)
            return;
        if (isHandFull)
        {

        }
        _axisX = horizontal;
        direction = horizontal;
    }
    private void Jump()
    {
        _rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    } 

    public void InitializeReset()
    {
        startPos = transform.position;
    }

    public void ResetAction()
    {
        transform.position = startPos;
    }
    #region 단순 편의성 기능
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position + (Vector3)footOffset, new Vector2(transform.localScale.x, 0.2f));
        Gizmos.DrawLine((Vector2)transform.position + handOffset, (Vector2)transform.position + handOffset + new Vector2(_direction, 0f));
    }
    #endregion
}
public enum AerialState
{
    None,
    Jump,
    Falling,
    Fallen,
}

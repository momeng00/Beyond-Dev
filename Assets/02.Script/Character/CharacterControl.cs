using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CharacterControl : MonoBehaviour, IReset, IDetected, IMovable
{
    public enum Direction
    {
        Left = -1, Right = 1,
    }
    #region 컴포넌트 선언부
    private Rigidbody2D _rb;
    private Animator _ani;
    #endregion
    [HideInInspector] public float _axisX; //이동되는 수치 받을 힘
    private Vector2 extraVelocity;
    private Direction _direction = Direction.Right;
    public Direction direction
    {
        get { return _direction; }
        set
        {
            if(_direction != value)
            {
                _direction = value;
                if (dust != null)
                    dust.Play();
                if (value == Direction.Right) 
                {
                    transform.localScale = new Vector3(1f, 1f, 1f);
                    //var test = dust.velocityOverLifetime;
                    //test.x = -0.4f;
                }
                else
                {
                    _direction = Direction.Left;
                    transform.localScale = new Vector3(-1f, 1f, 1f);
                    //var test = dust.velocityOverLifetime;
                    //test.x = 0.4f;
                }
                
            }
        }
    }
    public ParticleSystem dust;
    private Vector2 startPos;
    public Vector2 handOffset;
    public float handDistance;
    [SerializeField] private LayerMask handLayerMask;

    public bool isHandFull
    {
        get
        {
            Collider2D handObj = null;
            RaycastHit2D hit = Physics2D.Raycast((Vector2)transform.position + handOffset,
                                                    new Vector2((float)_direction,0f),
                                                    handDistance,
                                                    handLayerMask);
            Debug.DrawRay((Vector2)transform.position + handOffset, new Vector2((float)_direction, 0f) * handDistance, hit.collider ? Color.red : Color.green);
            handObj = hit.collider;
            if (handObj != null)
            {
                return !handObj.isTrigger;
            }
            return handObj;
            
        }
    }
    public bool isGrounded
    {
        get
        {
            _col = Physics2D.OverlapBox((Vector2)transform.position + footOffset,
                footSizeOffset,
                0.0f,
                groundMask);
//            if (_col != null)
   //         {
   //             return !_col.isTrigger;
        //    }
            return _col;
        }
    }
    public float jumpBuffetTime;
    [HideInInspector] public float jumpTime;
    [HideInInspector] public bool canJump;
    [HideInInspector] public bool hasJump;
    public float coyoteTime;
    public float landingLimit;
    [Header("카메라 이동 시 멈출시간")]
    public float stopTime;
    [HideInInspector] public bool canMove;

    public void StopCharacter()
    {
        StartCoroutine(IStopCharacter(stopTime));
    }
    private AerialState _aerialState;

    public List<Stat> stats = new List<Stat>();
    private Stat _currentStat;
    public Stat currentStat {
        get
        {
            if (_currentStat == null)
            {
                _currentStat = Instantiate(stats.FirstOrDefault());
                return _currentStat;
            }
            return _currentStat;
        }
    }

    private Dictionary<object, Vector2> _extraVelocityList = new Dictionary<object, Vector2>();
    public Dictionary<object, Vector2> ExtraVelocityList { get => _extraVelocityList; }

    public Vector2 footOffset;
    public Vector2 footSizeOffset;

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
        canMove = true;
        canJump = true;
        hasJump = false;
        _col = GetComponent<Collider2D>();
        _ani = GetComponent<Animator>();
        _rb = GetComponent<Rigidbody2D>();
        _axisX = 0;
    }
    // Update is called once per frame
    void Update()
    {
        if(jumpTime>-1)
            jumpTime -= Time.deltaTime;
    }
    private void FixedUpdate()
    {
        _rb.linearVelocity = new Vector2((_axisX) * currentStat.moveSpeed + extraVelocity.x, _rb.linearVelocityY + extraVelocity.y);
    }
    private void OnEnable()
    {
        //MainCameraController.Instance.Register(_rb);
    }

    private void OnDisable()
    {
        //MainCameraController.Instance.Unregister(_rb);
    }
    private void Move(float horizontal)
    {
        if(!canMove)
            return;
        _axisX = horizontal;
        if (horizontal > 0f)
        {
            direction = Direction.Right;
        }
        else if(0f > horizontal)
        { 
            direction = Direction.Left;
        }
        
    }
    public void ZeroVelocity()
    {
        _rb.linearVelocity = Vector2.zero;
    }
    private void Jump()
    {
        jumpTime = jumpBuffetTime;
        if (!canJump)
            return;
        if (hasJump)
            return;
        _rb.linearVelocity = Vector2.zero;
        _rb.AddForce(Vector2.up * currentStat.jumpForce, ForceMode2D.Impulse);
        if (dust != null)
            dust.Play();
    }

    public void InitializeReset()
    {
        startPos = transform.position;
    }

    public void ResetAction()
    {
        MainCameraController.Instance.CameraReset();
        _rb.linearVelocity = Vector2.zero;
        StartCoroutine("RespawnRoutine");
        AudioManager.Instance.PlaySFXAudio(AudioName.Die);
    }

    public void OnDetected()
    {
        throw new System.NotImplementedException();
    }
    public void OnDetected(CharacterStat stat)
    {
        Stat deSO = stats.Find(so => so.name == stat.ToString());
        if (deSO != null)
        {
            _currentStat = deSO;
        }
    }
    #region 단순 편의성 기능
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position + (Vector3)footOffset, footSizeOffset);
        Gizmos.DrawLine((Vector2)transform.position + handOffset, (Vector2)transform.position + handOffset + new Vector2((float)_direction*handDistance, 0f));
    }

    public void AddExtraVelocity(object root, Vector2 force)
    {
        _extraVelocityList[root]=force;
        extraVelocity = CalculateVelocity();
    }

    public void RemoveExtraVelocity(object root)
    {
        if (_extraVelocityList.ContainsKey(root))
        {
            _extraVelocityList.Remove(root);
        }
        extraVelocity = CalculateVelocity();
    }

    public Vector2 CalculateVelocity()
    {
        return _extraVelocityList.Values.Aggregate(Vector2.zero, (acc, v) => acc + v);
    }

    #endregion
    private IEnumerator RespawnRoutine()
    {
        // 1. 플레이어 이동
        transform.position = startPos;

        // 2. 아주 중요한 대기! (유니티가 이동을 처리할 시간을 줌)
        yield return null;

        // 3. 카메라 리셋
        MainCameraController.Instance.CameraReset();
    }

    private IEnumerator IStopCharacter(float time)
    {
        _rb.linearVelocity = Vector2.zero;
        _axisX = 0f;
        canMove = false;
        yield return new WaitForSeconds(time);
        canMove = true;
    }
}
public enum AerialState
{
    None,
    Jump,
    Falling,
    Fallen,
}

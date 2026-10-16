using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CharacterControl : MonoBehaviour, IReset, IDetected, IMovable
{
    private InputSystem _registeredInputSystem;
    private GameManager registeredGameManager;
    private readonly HashSet<object> pauseOwners = new HashSet<object>();
    private RigidbodyType2D bodyTypeBeforePause;
    private bool canMoveBeforePause;
    public bool IsPaused => pauseOwners.Count > 0;

    private void OnDestroy()
    {
        if (registeredGameManager != null)
        {
            registeredGameManager.UnregisterCharacter(this);
            registeredGameManager.initAction -= InitializeReset;
            registeredGameManager.OnReset -= ResetAction;
        }
        if (_registeredInputSystem == null)
            return;

        _registeredInputSystem.DeregisterAction(KeyState.Play_Key, "Horizontal", Move);
        _registeredInputSystem.DeregisterAction(KeyState.Play_Key, KeyCode.Space, Jump);

        _registeredInputSystem = null;
    }

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
    private Collider2D ground;
    public bool isGrounded
    {
        get
        {
            ground = Physics2D.OverlapBox((Vector2)transform.position + footOffset,
                footSizeOffset,
                0.0f,
                groundMask);
            return ground;
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

    private readonly object respawnPauseOwner = new object();
    public bool IsRespawning { get; private set; }

    private Collider2D _col;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Initialize();
    }
    private void Initialize()
    {
        _registeredInputSystem = InputSystem.Instance;
        _registeredInputSystem.RegisterAction(KeyState.Play_Key, "Horizontal", Move);
        _registeredInputSystem.RegisterAction(KeyState.Play_Key, KeyCode.Space, Jump);
        registeredGameManager = GameManager.Instance;
        registeredGameManager.RegisterInitAction(InitializeReset);
        registeredGameManager.OnReset += ResetAction;
        InitializeReset();
        registeredGameManager.RegisterCharacter(this);
        
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
        if (!InputSystem.Instance.CanProcessGameplayInput)
        {
            _axisX = 0f;
            jumpTime = -1f;
        }
        if (IsPaused || !canMove)
            return;
        _rb.linearVelocity = new Vector2((_axisX) * currentStat.moveSpeed + extraVelocity.x, _rb.linearVelocityY + extraVelocity.y);
    }
    private void OnEnable()
    {
        if (registeredGameManager != null)
            registeredGameManager.RegisterCharacter(this);
        //MainCameraController.Instance.Register(_rb);
    }

    private void OnDisable()
    {
        if (IsRespawning)
        {
            if (RespawnCoroutine != null)
                StopCoroutine(RespawnCoroutine);
            FinishRespawn();
        }
        //MainCameraController.Instance.Unregister(_rb);
    }
    private void Move(float horizontal)
    {
        if (IsPaused || !canMove)
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
        if (IsPaused)
        {
            jumpTime = -1f;
            return;
        }
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
    public void PauseCharacter()
    {
        PauseCharacter(this);
    }

    public void PauseCharacter(object owner)
    {
        if (owner == null || !pauseOwners.Add(owner)) return;
        if (pauseOwners.Count == 1)
        {
            bodyTypeBeforePause = _rb.bodyType;
            canMoveBeforePause = canMove;
            ZeroVelocity();
            _rb.bodyType = RigidbodyType2D.Static;
        }
        canMove = false;
        _axisX = 0f;
        jumpTime = -1f;
    }

    public void UnPauseCharacter()
    {
        UnPauseCharacter(this);
    }

    public void UnPauseCharacter(object owner)
    {
        if (owner == null || !pauseOwners.Remove(owner) || IsPaused) return;
        _rb.bodyType = bodyTypeBeforePause;
        ZeroVelocity();
        _axisX = 0f;
        jumpTime = -1f;
        canMove = canMoveBeforePause;
    }
    public void InitializeReset()
    {
        startPos = transform.position;
    }

    public Coroutine RespawnCoroutine { get; private set; }

    public void ResetAction()
    {
        if (IsRespawning || !isActiveAndEnabled)
            return;

        RespawnCoroutine = StartCoroutine(RespawnRoutine());
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
        IsRespawning = true;
        try
        {
            PauseCharacter(respawnPauseOwner);

            // 바닥 보정 성공 여부와 관계없이 기존 부활 위치로 먼저 복귀한다.
            transform.position = startPos;

            // 같은 OnReset 호출의 다른 구독자가 실행된 뒤 검사한다.
            yield return null;
            Physics2D.SyncTransforms();

            if (!TryPlaceOnRespawnGround(out string failureReason))
            {
                //NeedToDo 바닥을 못 찾거나 몸이 벽에 겹칠 때 사용할 대체 부활 위치 구현해야함
                Debug.LogWarning($"{name}: 부활 위치의 바닥 보정에 실패했습니다. {failureReason} 저장된 시작 위치에서 기본 부활을 진행합니다.", this);
            }

            _rb.linearVelocity = Vector2.zero;
            _axisX = 0f;
            jumpTime = -1f;
            _extraVelocityList.Clear();
            extraVelocity = Vector2.zero;
            hasJump = false;
            canJump = true;

            if (TryGetComponent(out CharacterAnimation stateMachine))
                stateMachine.ChangeState(CharacterStateID.Idle);

            yield return null;
            MainCameraController.Instance.CameraReset();
        }
        finally
        {
            FinishRespawn();
        }
    }

    private bool TryPlaceOnRespawnGround(out string failureReason)
    {
        failureReason = "탐색 거리 내에서 groundMask에 해당하는 바닥을 찾지 못했습니다.";
        if (_col == null || !_col.enabled || _col.isTrigger || !_rb.simulated)
        {
            failureReason = "캐릭터 Collider2D의 활성화/Trigger 또는 Rigidbody2D의 Simulated 설정을 확인하세요.";
            return false;
        }

        ContactFilter2D groundFilter = new ContactFilter2D();
        groundFilter.SetLayerMask(groundMask);
        groundFilter.useTriggers = false;
        Vector2 groundOrigin = new Vector2(startPos.x + footOffset.x, startPos.y);
        RaycastHit2D hit = Physics2D.defaultPhysicsScene.Raycast(
            groundOrigin, Vector2.down, 10f, groundFilter);
        if (hit.collider == null)
            return false;

        if (hit.collider.attachedRigidbody == _rb || Physics2D.GetIgnoreCollision(_col, hit.collider))
        {
            failureReason = "검출된 첫 콜라이더는 캐릭터 자신이거나 충돌을 무시하도록 설정된 대상입니다.";
            return false;
        }

        if (hit.fraction <= 0f)
        {
            failureReason = $"Raycast 시작점이 {hit.collider.name} 내부 또는 표면에 있습니다.";
            return false;
        }
        if (hit.normal.y <= 0f)
        {
            failureReason = $"{hit.collider.name}에서 설 수 있는 바닥 윗면을 찾지 못했습니다.";
            return false;
        }

        Vector3 previousPosition = transform.position;
        // 발밑 검사 영역의 중심을 바닥 표면에 맞춘다.
        float respawnY = hit.point.y - footOffset.y;
        Vector3 respawnPosition = new Vector3(startPos.x, respawnY, previousPosition.z);

        if (!IsRespawnPositionClear(respawnPosition, out failureReason))
            return false;

        transform.position = respawnPosition;
        Physics2D.SyncTransforms();
        if (isGrounded)
        {
            failureReason = null;
            return true;
        }

        failureReason = "배치 후 isGrounded가 false입니다. footOffset, footSizeOffset, 콜라이더 크기를 확인하세요.";
        // 발밑 판정에 실패하면 검사 전 시작 위치로 되돌린다.
        transform.position = previousPosition;
        Physics2D.SyncTransforms();
        ground = null;
        return false;
    }

    private bool IsRespawnPositionClear(Vector3 respawnPosition, out string failureReason)
    {
        failureReason = null;
        ContactFilter2D bodyFilter = new ContactFilter2D();
        bodyFilter.SetLayerMask(groundMask);
        bodyFilter.useTriggers = false;

        Bounds expectedBounds = _col.bounds;
        expectedBounds.center += respawnPosition - transform.position;
        Collider2D candidate = Physics2D.defaultPhysicsScene.OverlapBox(
            expectedBounds.center, expectedBounds.size, 0f, bodyFilter);
        if (candidate == null)
            return true;

        // 겹침 가능성이 있을 때만 결과 목록을 만들고 실제 몸체 모양으로 확인한다.
        List<Collider2D> overlaps = new List<Collider2D>(8);
        Vector2 bodyPosition = _rb.position + (Vector2)(respawnPosition - transform.position);
        int overlapCount = _col.Overlap(bodyPosition, _rb.rotation, bodyFilter, overlaps);
        for (int i = 0; i < overlapCount; i++)
        {
            Collider2D other = overlaps[i];
            if (other == _col || other.attachedRigidbody == _rb || Physics2D.GetIgnoreCollision(_col, other))
                continue;

            failureReason = $"부활 위치에서 캐릭터가 {other.name}과 겹칩니다.";
            return false;
        }

        return true;
    }

    private void FinishRespawn()
    {
        IsRespawning = false;
        RespawnCoroutine = null;
        UnPauseCharacter(respawnPauseOwner);
    }

    private IEnumerator IStopCharacter(float time)
    {
        if (IsPaused) canMoveBeforePause = false;
        _rb.linearVelocity = Vector2.zero;
        _axisX = 0f;
        canMove = false;
        yield return new WaitForSeconds(time);
        if (IsPaused)
            canMoveBeforePause = true;
        else
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

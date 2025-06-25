using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.UI.Image;

public class CharacterControl : MonoBehaviour, IReset
{
    
    private Rigidbody2D _rb;
    private Animator _ani;
    
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

        if (isHandFull)
        {
        }
    }
    private void FixedUpdate()
    {
        _rb.linearVelocity = new Vector2(_axisX * moveSpeed , _rb.linearVelocityY);
    }
    private void Move(float horizontal)
    {
        if(!_canMove)
            return;
        _axisX = horizontal;
        direction = horizontal;
    }
    private void Jump()
    {
        _rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    } 

    //private IDetect OnDetect()
    //{
    //    Collider2D[] hitColliders = Physics2D.OverlapCircleAll(_rb.position, detectRange, detectMask);

    //    IDetect currentClosest = null;
    //    float minDistanceSqr = float.MaxValue; // 제곱 거리를 사용하면 Mathf.Sqrt 계산을 피할 수 있어 성능에 좋습니다.

    //    // 2. 감지된 Collider2D들 중에서 IDetect 인터페이스를 가진 오브젝트를 찾습니다.
    //    foreach (Collider2D hitCollider in hitColliders)
    //    {
    //        // 자기 자신은 제외 (선택 사항)
    //        if (hitCollider.gameObject == this.gameObject)
    //        {
    //            continue;
    //        }

    //        // TryGetComponent를 사용하여 Collider2D의 GameObject가 IDetect 인터페이스를 가지고 있는지 확인합니다.
    //        if (hitCollider.TryGetComponent<IDetect>(out IDetect detectedObject))
    //        {
    //            // 3. IDetect를 가진 오브젝트 중 가장 가까운 것을 찾습니다.
    //            float distanceSqr = ((Vector2)hitCollider.transform.position - _rb.position).sqrMagnitude;

    //            if (distanceSqr < minDistanceSqr)
    //            {
    //                minDistanceSqr = distanceSqr;
    //                currentClosest = detectedObject;
    //            }
    //        }
    //    }
    //    if (currentClosest != null)
    //    {
    //        if (_detect != null)//무언가 이미 감지되어있었을때
    //        {
    //            if (_detect == currentClosest) //같은거일때
    //            {
    //                return _detect;
    //            }
    //            _detect.DetectExit();
    //            currentClosest.DetectAction(this.gameObject);
    //            currentClosest.DetectEnter();
    //            _detect = currentClosest;
    //        }
    //        else //무언가 감지된적 없을때
    //        {
    //            currentClosest.DetectAction(this.gameObject);
    //            currentClosest.DetectEnter();
    //            _detect = currentClosest;
    //            return currentClosest;
    //        }
    //    }
    //    else
    //    {
    //        if (_detect != null)//무언가 이미 감지되어있었을때
    //        {
    //            _detect.DetectExit();
    //        }
    //    }
    //    return currentClosest;
    //}

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position + (Vector3)footOffset, new Vector2(transform.localScale.x, 0.2f));
        Gizmos.DrawLine((Vector2)transform.position + handOffset, (Vector2)transform.position + handOffset + new Vector2(_direction, 0f));
    }

    public void InitializeReset()
    {
        startPos = transform.position;
    }

    public void ResetAction()
    {
        transform.position = startPos;
    }
}
public enum AerialState
{
    None,
    Jump,
    Falling,
    Fallen,
}

using UnityEngine;

public class CharacterControl : MonoBehaviour
{
    
    private Rigidbody2D _rb;
    private Animator _ani;
    
    private float _axisX; //이동되는 수치 받을 힘
    private float _axisY; //이동되는 수치 받을 힘
    private float _direction;
    private Vector2 _prePos;
    public bool isGrounded
    {
        get
        {
            Collider2D col;
            col = Physics2D.OverlapBox(_rb.position + footOffset, 
                new Vector2(transform.localScale.x, 0.2f),
                0.0f,
                groundMask);
            return col;
        }
    }

    private bool _canMove;
    private bool _isDetected;

    private AerialState _aerialState;

    public float moveSpeed;
    public float jumpForce;
    public Vector2 footOffset;
    public LayerMask detectMask;
    public LayerMask groundMask;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Initialize();
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
        _axisX = horizontal;
    }
    private void Jump()
    {
        _rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }

    private void Initialize()
    {
        InputSystem.Instance.RegisterAction(KeyState.Play_Key,"Horizontal",Move);
        InputSystem.Instance.RegisterAction(KeyState.Play_Key, KeyCode.Space, Jump);
    }

}
public enum AerialState
{
    None,
    Jump,
    Falling,
    Fallen,
}

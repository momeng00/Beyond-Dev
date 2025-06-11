using UnityEngine;

public class CharacterControl : MonoBehaviour
{
    
    private Rigidbody2D _rb;
    private Animator _ani;

    private float _axisX;
    private float _axisY;
    private Vector2 _move;
    private Vector2 _prePos;

    private bool _isGrounded;
    private bool _canMove;
    private bool _isDetected;

    private AerialState _aerialState;

    public LayerMask detectMask;
    public LayerMask groundMask;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void Move()
    {
        if(!_canMove)
            return;

    }
}
public enum AerialState
{
    None,
    Jump,
    Falling,
    Fallen,
}

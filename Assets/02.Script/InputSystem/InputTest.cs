using System.Collections.Generic;
using UnityEngine;
public class InputTest : MonoBehaviour
{
    private float _x;
    private Rigidbody2D rb;
    private Animator ani;
    public Vector2 vec;
    public CharacterState characterState;
    public float jumpForce;
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        ani = GetComponent<Animator>();
        initialize();
    }
    private void Update()
    {
        rb.position += new Vector2(_x*3.2f*Time.deltaTime,0f);
        if (rb.linearVelocityY > 0.05f)
        {
            ani.SetInteger("State",1);
            characterState = CharacterState.Jump;
        }
        else if (rb.linearVelocityY < -0.005f)
        {
            if (characterState == CharacterState.Falling)
                return;
            ani.SetInteger("State", 2);
            characterState = CharacterState.Falling;
            vec.y = transform.position.y;
        }
        else if (vec.y - transform.position.y > 8.0f)
        {
            ani.SetInteger("State", 3);
            characterState = CharacterState.Fallen;
        }
        else
        {
            ani.SetInteger("State", 0);
            vec.y = Vector2.zero.y;
            characterState = CharacterState.Ground;
        }

    }
    public void initialize()
    {
        InputSystem.Instance.RegisterAction(KeyState.Play_Key,"Horizontal",Move);
        InputSystem.Instance.RegisterAction(KeyState.Play_Key, KeyCode.Space, Jump);
    }
    public void Move(float x)
    {
        _x = x;
    }
    public void Jump()
    {
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }
      
}
public enum CharacterState
{
    Ground,
    Jump,
    Falling,
    Fallen,
}

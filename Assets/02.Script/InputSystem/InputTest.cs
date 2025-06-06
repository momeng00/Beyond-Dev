using System.Collections.Generic;
using UnityEngine;
public class InputTest : MonoBehaviour
{
    public float _x;
    public Rigidbody2D rb;
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        initialize();
    }
    private void Update()
    {
        rb.position += new Vector2(_x*3.2f*Time.deltaTime,0.0f);
    }
    public void initialize()
    {
        InputSystem.Instance.RegisterAction(KeyState.Play_Key,"Horizontal",Move);
    }
    public void Move(float x)
    {
        _x = x;
    }
        
}

using UnityEngine;

public class Object : MonoBehaviour, IDetect, IReset
{
    private Collider2D col;
    private Rigidbody2D rb;

    public void DetectAction()
    {
        throw new System.NotImplementedException();
    }

    public void DetectAction(GameObject sender)
    {
        throw new System.NotImplementedException();
    }

    public void DetectEnter()
    {
        throw new System.NotImplementedException();
    }

    public void DetectExit()
    {
        throw new System.NotImplementedException();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        
    }
    private void OnCollisionExit(Collision collision)
    {
        
    }
}
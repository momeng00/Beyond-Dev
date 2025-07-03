using System.Collections.Generic;
using UnityEngine;

public class SingleTeleport : Spot
{
    public Transform arrivePos;
    private Collider2D col;


    private void Start()
    {
        col = GetComponent<Collider2D>();
        col.isTrigger = true;
    }

    public override void Interact()
    {
        base.Interact();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (IsInLayerMask(collision.gameObject, layerMask))
        {
            collision.gameObject.transform.position = arrivePos.position;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube((Vector3)arrivePos.position, new Vector3(0.5f, 0.5f, 0));
    }
}
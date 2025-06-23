using System.Collections.Generic;
using UnityEngine;

public class SwitchPhone : Switch
{
    public List<Block> targetBlock = new List<Block>();
    private bool isSatisfied;
    public override void Interact()
    {
        base.Interact();
        if (isSatisfied)
        {
            foreach (var block in targetBlock)
            {
                block.OnBlockAction();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (IsInLayerMask(collision.gameObject,layerMask))
        {
            isSatisfied = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (IsInLayerMask(collision.gameObject, layerMask))
        {
            isSatisfied = false;    
        }
    }
}
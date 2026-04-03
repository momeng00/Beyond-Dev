using System;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    [Header("트리거 액션인가 그거")]
    public Collider2D doorCollider;

    public List<OpenDoorItem> satisfiedItem;

    private void Start()
    {
        doorCollider.enabled = false;
        foreach (OpenDoorItem item in satisfiedItem)
        {
            item.AddOnCheck(LoveIsOpenDoor);
        }
    }

    public bool IsSatisfied()
    {
        foreach (OpenDoorItem item in satisfiedItem)
        {
            if (!item.IsSatisfied())
            {
                return false;
            }
        }
        return true;
    }
    public void LoveIsOpenDoor()
    {
        if (IsSatisfied())
        {
            doorCollider.enabled = true;
        }
        else
        {
            doorCollider.enabled = false;
        }
    }

}
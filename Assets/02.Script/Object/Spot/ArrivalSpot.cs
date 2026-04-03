using UnityEngine;

public class ArrivalSpot : Spot
{
    public Transform target;
    public void ArrivalTarget()
    {
        target.transform.position = this.GetComponent<Transform>().transform.position;
    }
}
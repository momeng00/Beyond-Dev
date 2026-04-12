using UnityEngine;

public class ArrivalSpot : Spot
{
    public Transform target;
    public void ArrivalTarget()
    {
        CharacterControl controller = target.GetComponent<CharacterControl>();
        if (controller != null)
        {
            controller.ZeroVelocity();
        }
        target.transform.position = this.GetComponent<Transform>().transform.position;
    }
}
using UnityEngine;

public class StatPlatform : Platform
{
    public CharacterStat changeStat;
    private void Start()
    {
        col = GetComponent<Collider2D>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (IsInLayerMask(collision.gameObject, layerMask))
        {

            IDetected detected = collision.gameObject.GetComponent<MonoBehaviour>() as IDetected;
            if (detected != null)
            {
                detected.OnDetected(changeStat);
            }
        }
        
    }
    private void OnCollisionExit2D(Collision2D collision)
    {

    }
}

using UnityEngine;

public class CamTest : MonoBehaviour
{
    public Transform player1;
    public Transform player2;
    public Transform cam;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (player1 != null && player2 != null)
        {
            cam.position = new Vector3(
                (player1.position.x + player2.position.x)/2
                , (player1.position.y + player2.position.y)/2
                , cam.position.z);
        }
    }
}

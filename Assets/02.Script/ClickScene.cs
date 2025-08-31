using UnityEngine;
using UnityEngine.SceneManagement;

public class ClickScene : MonoBehaviour
{
    float mustTime = 10f;
    public float t;
    void Start()
    {
        t = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        t += Time.deltaTime;
        if (t > mustTime)
        {
            if (Input.anyKey)
            {
                SceneManager.LoadScene("Title");
            }
        }
    }
}

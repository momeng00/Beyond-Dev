using UnityEngine;
using UnityEngine.SceneManagement;

public class ClickScene : MonoBehaviour
{
    float mustTime = 3f;
    float t;
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
                SceneManager.LoadScene("Build_Level (0) 1");
            }
        }
    }
}

using UnityEngine;
using UnityEngine.Rendering;

public class TestBlur : MonoBehaviour
{
    public Volume volume;
    GaussianBlur1DVolumeComponent blur;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (volume.profile.TryGet(out blur))
        {
            Debug.Log("Blur 찾음");
            Debug.Log(blur);
        }
        else
        {
            Debug.Log("Blur 못 찾음");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            blur.radius.value = 0;
            blur.dimmed.value = 0;
            Debug.Log("pressQ");
        }
        if (Input.GetKeyDown(KeyCode.F2))
        {
            blur.radius.value = 1;
            blur.dimmed.value = 1;
            Debug.Log("pressF2");
        }
    }
}

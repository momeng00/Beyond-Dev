using UnityEngine;
using UnityEngine.Rendering;

public class TempVolume : MonoBehaviour
{
    private Volume globalVolume;
    public VolumeProfile changeVolume;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        globalVolume = GetComponent<Volume>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            globalVolume.profile = changeVolume;
            Debug.Log(globalVolume + " : " + changeVolume);
        }
    }
}

using UnityEngine;

public class TestUI : MonoBehaviour
{
    public UIWindow window;
    public UIWindow second;

    private void Start()
    {
        window.Open();
        second.Open();
        second.Close();

    }
    private void Update()
    {
        
    }
}
using UnityEngine;

public class TestUI : MonoBehaviour
{
    public UIWindow window;
    public UIWindow second;
    private void Awake()
    {
        window.Open();
        second.Open();
    }
    private void Start()
    {
        second.Close();
    }
    private void Update()
    {
        
    }
}
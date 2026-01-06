using UnityEngine;

public class TestUI : MonoBehaviour
{
    public UIBase uiBase;
    public UIWindow window;
    public UIWindow second;

    private void Start()
    {
        window.Open();
        second.Close();
    }
    private void Update()
    {
        
    }
}
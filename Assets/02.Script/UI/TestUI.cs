using UnityEngine;

public class TestUI : MonoBehaviour
{
    public UIBase uiBase;
    public UIWindow window;
    public UIWindow second;

    private void Start()
    {
        
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            window.Open();
        }
    }
}
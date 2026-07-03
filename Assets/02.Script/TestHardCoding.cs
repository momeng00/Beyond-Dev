using UnityEngine;

public class TestHardCoing : MonoBehaviour
{
    public UIBaseUpgrade test1;
    public UIBaseUpgrade test2;
    public UIBaseUpgrade test3;
    public UIBaseUpgrade test4;
    public UIBaseUpgrade test5;
    public UIBaseUpgrade test6;
    public UIBaseUpgrade test7;
    public UIBaseUpgrade test8;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Q))
        {
            test1.Open();

        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            test2.Open();
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            test2.Close();

        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            test1.Close();

        }
        if (Input.GetKeyDown(KeyCode.T))
        {
            test5.Open();

        }
        if (Input.GetKeyDown(KeyCode.Y))
        {
            test6.Open();

        }
        if (Input.GetKeyDown(KeyCode.U))
        {
            test7.Open();

        }
        if (Input.GetKeyDown(KeyCode.I))
        {
            test8.Open();

        }
    }
}

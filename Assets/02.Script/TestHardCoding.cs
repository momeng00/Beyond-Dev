using UnityEngine;

public class TestHardCoing : MonoBehaviour
{
    public Card test1;
    public Card test2;
    public Card test3;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        test1.RemoveCard();
        test2.RemoveCard();
        test3.RemoveCard();
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Q))
        {

            CardManager.Instance.AddCard(test1);
        }
        if (Input.GetKeyDown(KeyCode.W))
        {

            CardManager.Instance.AddCard(test2);
        }
        if (Input.GetKeyDown(KeyCode.E))
        {

            CardManager.Instance.AddCard(test3);
        }
    }
}

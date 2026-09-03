using TMPro;
using UnityEngine;

public class TestHardCoing : MonoBehaviour
{
    public Card card1;
    public Card card2;
    public Card card3;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameManager.Instance.StartGameNow();
        
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Q))
        {
            CardManager.Instance.AddCard(card1);
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            CardManager.Instance.AddCard(card2);
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            CardManager.Instance.AddCard(card3);

        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            CardManager.Instance.HideCardList();
        }
        if (Input.GetKeyDown(KeyCode.T))
        {
            CardManager.Instance.ShowTopCard();

        }
        if (Input.GetKeyDown(KeyCode.Y))
        {
          

        }
        if (Input.GetKeyDown(KeyCode.U))
        {
          
        }
        if (Input.GetKeyDown(KeyCode.I))
        {
           

        }
    }
}

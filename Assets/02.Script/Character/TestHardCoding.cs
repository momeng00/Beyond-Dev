using UnityEngine;

public class TestHardCoing : MonoBehaviour
{
    public Card test;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.Q))
        {
            Card clone = Instantiate(test, test.gameObject.transform.parent);
            CardManager.Instance.AddCard(clone);
        }
    }
}

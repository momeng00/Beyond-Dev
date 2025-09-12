using UnityEngine;

public class PressE : MonoBehaviour
{
    public SwitchPhone master;
    public bool masterSatisfied;
    private void Awake()
    {
        master = GetComponentInParent<SwitchPhone>();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameObject.GetComponent<SpriteRenderer>().enabled = master.IsSatisfied;
        masterSatisfied = master.IsSatisfied;
    }

    // Update is called once per frame
    void Update()
    {
        if (masterSatisfied != master.IsSatisfied)
        {
            masterSatisfied = master.IsSatisfied;
            gameObject.GetComponent<SpriteRenderer>().enabled = master.IsSatisfied;
        }
    }
}

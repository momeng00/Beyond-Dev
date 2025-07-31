using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MultiSwitch : MonoBehaviour
{
    [Header("ÇÊ¼öÀû")]
    public List<Block> switchs;
    public TMP_Text numLeft;

    [Space(15f)]
    private Collider2D col;

    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        numLeft.text = switchs.Count.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void CheckLeftBlock()
    {
        int leftNum=0; 
        foreach (var left in switchs)
        {
            if (!left.gameObject.activeSelf)
            {
                leftNum++;
            }
        }
        numLeft.text = numLeft.ToString();
    }
}

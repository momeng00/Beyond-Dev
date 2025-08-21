using TMPro;
using UnityEngine;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

public class MessageBlock : Block, ISwitchable
{
    public TextMeshPro text;
    public GameObject emoji;
    private BoxCollider2D boxCollider;
    public Switch Switch => throw new System.NotImplementedException();

    public void SwitchOn(bool value)
    {
        throw new System.NotImplementedException();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void Start()
    {
        base.Start();
        GameObject textObj = new GameObject("Text");

        // 위치 초기화
        textObj.transform.localRotation = Quaternion.identity;
        textObj.transform.localScale = Vector3.one;
        textObj.transform.localPosition = transform.position;
        // TextMeshPro 컴포넌트 추가
        TextMeshPro tmp = textObj.AddComponent<TextMeshPro>();


        // 텍스트 설정
        tmp.text = "Hello World!";
        
        tmp.fontSize = 3;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.enableAutoSizing = false;
        tmp.color = Color.black;
        boxCollider = gameObject.AddComponent<BoxCollider2D>();
        boxCollider.size = GetComponent<SpriteRenderer>().sprite.bounds.size;

        RectTransform rectTransform = tmp.rectTransform;
        Vector2 spriteWorldSize = GetComponent<SpriteRenderer>().bounds.size;

        Vector2 localSize = rectTransform.InverseTransformVector(spriteWorldSize);
        rectTransform.sizeDelta = localSize;

        textObj.transform.SetParent(transform);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

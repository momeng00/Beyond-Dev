using TMPro;
using UnityEngine;

public class TagContent : MonoBehaviour
{
   
    public RectTransform body;
    public SpriteRenderer rendererBody;

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {

            SetBody();
        }
    }
    public void SetBody()
    {
        rendererBody.size= new Vector2(rendererBody.size.x, body.rect.height + 0.1f);
    }
}
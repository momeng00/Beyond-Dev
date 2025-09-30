using TMPro;
using UnityEngine;

public class TagContent : MonoBehaviour
{
   
    public RectTransform body;
    public SpriteRenderer rendererBody;

    public void SetBody()
    {
        rendererBody.size= new Vector2(body.rect.width, body.rect.height + 0.1f);
    }
}
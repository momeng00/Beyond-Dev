using UnityEngine;

public class DownloadStation : Spot
{
    private Material material;
    private SpriteRenderer spriteRenderer;
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        material = spriteRenderer.material;
    }
    public void GetPartnerDate(Vector2 position, Vector2 size)
    {
        material.SetVector("_UploadStation_Pos", position);
        material.SetVector("_UploadStation_Size", size);
        spriteRenderer.size = size;
        Debug.Log(position);
        Debug.Log(size);
    }
}
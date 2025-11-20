using NUnit.Framework;
using UnityEngine;

public class DownloadStation : MonoBehaviour
{
    private Material material;
    private SpriteRenderer spriteRenderer;
    [SerializeField]private UploadStation partnerStation;
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
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            partnerStation.GetDetectedObject(this.gameObject);
        }
    }
}
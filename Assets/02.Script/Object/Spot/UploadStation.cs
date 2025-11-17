using System.Collections.Generic;
using UnityEngine;

public class UploadStation : Spot, ISwitchable
{
    public List<DownloadStation> partnerStations;
    private BoxCollider2D col;
    private Animator ani;
    private Material material;
    private SpriteRenderer spriteRenderer;
    public Switch Switch => throw new System.NotImplementedException();
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        material = spriteRenderer.material;
        material.SetVector("_Size", spriteRenderer.size);
        partnerInit();
        //col = GetComponent<BoxCollider2D>();
        //ani = GetComponent<Animator>();
    }
    public void SwitchOn(bool value)
    {
        throw new System.NotImplementedException();
    }
    public void partnerInit()
    {
        foreach(DownloadStation partner in partnerStations)
        {
            if(partner != null)
            {
                partner.GetPartnerDate(transform.position, spriteRenderer.size);
            }
        }
    }
}
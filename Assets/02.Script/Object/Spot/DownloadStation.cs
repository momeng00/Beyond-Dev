using System.Collections.Generic;
using UnityEngine;

public class DownloadStation : MonoBehaviour, ISwitchable
{
    private Material material;
    private SpriteRenderer spriteRenderer;
    [SerializeField]private UploadStation partnerStation;
    public List<DownloadStationSwtich> switches;

    public Switch Switch => throw new System.NotImplementedException();

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        material = spriteRenderer.material;
    }
    private void Start()
    {
        foreach (Switch sw in switches)
        {
            sw.SetSwitch(this);
        }
    }
    public void GetPartnerDate(Vector2 position, Vector2 size)
    {
        material.SetVector("_UploadStation_Pos", position);
        material.SetVector("_UploadStation_Size", size);
        spriteRenderer.size = size;
    }

    public void SwitchOn(bool value)
    {
        if (value)
        {
            partnerStation.GetDetectedObject(this.gameObject);
        }
        else
        {
            partnerStation.PoolingReturn();
        }
    }

    public void UploadComplete(bool value)
    {
        foreach (DownloadStationSwtich dss in switches)
        {
            dss.isUpload = value;
            dss.ani.SetBool("activate", value);
        }
    }
}
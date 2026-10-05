using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DownloadStation : MonoBehaviour, ISwitchable
{
    private Material material;
    private SpriteRenderer spriteRenderer;
    [SerializeField]private UploadStation partnerStation;
    public List<DownloadStationSwtich> switches;
    public List<GameObject> blocks;
    private Animator ani;
    private bool canRecall = false;
    public Switch Switch => throw new System.NotImplementedException();

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        material = spriteRenderer.material;
        ani = GetComponent<Animator>();
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

    public bool SwitchOn(bool value)
    {
        foreach (GameObject block in blocks)
        {
            block.GetComponent<Collider2D>().enabled = true;
            block.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
        }
        ani.SetBool("activate", false);
        //if (canRecall)
        //{
        //    partnerStation.PoolingReturn();
        //    ani.SetBool("activate", true); 
        //    canRecall = false;
        //}
        //else
        //{
        //    partnerStation.GetDetectedObject(this.gameObject);
        //    foreach (GameObject block in blocks)
        //    {
        //        block.GetComponent<Collider2D>().enabled = true;
        //        block.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
        //    }
        //    ani.SetBool("activate", false);
        //    canRecall = true;
        //}
        return true;
    }
    public void RefreshVisuals(bool state)
    {
        ani.SetBool("activate", state);
        canRecall = !state;
    }

    public void UploadComplete(bool value)
    {
        ani.SetBool("activate", value);
        foreach (DownloadStationSwtich dss in switches)
        {
            dss.isUpload = value;
            dss.anime.SetBool("activate", value);
        }
    }


}
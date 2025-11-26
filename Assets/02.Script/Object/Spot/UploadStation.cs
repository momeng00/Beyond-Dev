using System.Collections.Generic;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEditor.PackageManager.Requests;
using UnityEngine;

public class UploadStation : Spot, ISwitchable, IReset
{
    public List<DownloadStation> partnerStations;
    public List<Switch> switches;
    private BoxCollider2D col;
    private Animator ani;
    private Material material;
    private SpriteRenderer spriteRenderer;
    private bool stationState;
    [SerializeField]private List<GameObject> detectedList = new List<GameObject>();
    [SerializeField]private List<GameObject> activeList = new List<GameObject>();
    private Dictionary<GameObject, GameObject> readyList = new Dictionary<GameObject, GameObject>();
    private bool isUploading = false;

    public Switch Switch => throw new System.NotImplementedException();
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        material = spriteRenderer.material;
        material.SetVector("_Size", spriteRenderer.size);
        col = GetComponent<BoxCollider2D>();
        ani = GetComponentInChildren<Animator>();
    }
    private void Start()
    {
        partnerInit();
        foreach (Switch sw in switches)
        {
            sw.SetSwitch(this);
        }
        col.isTrigger = true;
        col.size = spriteRenderer.size;
        stationState = false;
        GameManager.Instance.OnReset += ResetAction;
    }

    public void SwitchOn(bool value)
    {
        stationState = value;
        isUploading = true;
        if (value)
        {
            foreach (GameObject ob in detectedList)
            {
                ob.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
                ob.GetComponent<Collider2D>().enabled = false;
            }
        }
        else
        {
            PoolingReturn();
            foreach (GameObject ob in detectedList)
            {
                ob.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
                ob.GetComponent<Collider2D>().enabled = true;
            }
        }
        foreach (DownloadStation ds in partnerStations)
        {
            ds.UploadComplete(value);
        }
        isUploading = false;
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

    public void GetDetectedObject(GameObject requester)
    {
        foreach (GameObject ob in detectedList)
        {
            Vector3 comparative = transform.position - ob.transform.position;
            GameObject clone = PoolingGet(ob);
            if(!clone.activeSelf)
                clone.SetActive(true);
            clone.transform.position = requester.transform.position - comparative;
            if (!activeList.Contains(clone))
            {
                activeList.Add(clone);
            }
            
        }
        
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isUploading) return;
        if (IsInLayerMask(collision.gameObject, layerMask) && !detectedList.Contains(collision.gameObject))
        {
            collision.gameObject.GetComponent<Block>().OnBlockAction();
            detectedList.Add(collision.gameObject);
        }
        if (detectedList.Count > 0)
        {
            ani.SetBool("IsDetected", true);
        }

    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (isUploading) return;
        if (detectedList.Contains(collision.gameObject))
        {
            detectedList.Remove(collision.gameObject);

        }
        if (detectedList.Count <= 0)
        {
            ani.SetBool("IsDetected", false);
        }
    }
    private GameObject PoolingGet(GameObject original)
    {
        if (!readyList.ContainsKey(original))
        {
            GameObject clone = Instantiate(original);
            readyList[original] = clone;
            clone.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
            clone.GetComponent<Collider2D>().enabled = true;
        }
        return readyList[original];
    }
    public void PoolingReturn()
    {

        foreach(GameObject ob in activeList)
        {
            if (ob.TryGetComponent(out PushBlock pushBlock))
            {
                pushBlock.UDAnimationPlay(stationState);
            }
        }
        activeList.Clear();
    }

    public void InitializeReset()
    {
        
    }

    public void ResetAction()
    {
        SwitchOn(false);
    }
}
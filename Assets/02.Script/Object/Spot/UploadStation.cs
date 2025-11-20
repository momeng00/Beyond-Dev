using System.Collections.Generic;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEditor.PackageManager.Requests;
using UnityEngine;

public class UploadStation : Spot, ISwitchable
{
    public List<DownloadStation> partnerStations;
    private BoxCollider2D col;
    private Animator ani;
    private Material material;
    private SpriteRenderer spriteRenderer;
    private bool stationState;
    private List<GameObject> detectedList;
    private List<GameObject> activeList;
    private Dictionary<GameObject, GameObject> readyList;
    private bool isUploading = false;

    public Switch Switch => throw new System.NotImplementedException();
    private void Awake()
    {
        detectedList = new List<GameObject>();
        activeList = new List<GameObject>();
        readyList = new Dictionary<GameObject, GameObject>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        material = spriteRenderer.material;
        material.SetVector("_Size", spriteRenderer.size);
        col = GetComponent<BoxCollider2D>();
        ani = GetComponent<Animator>();
    }
    private void Start()
    {
        partnerInit();
        col.isTrigger = true;
        col.size = spriteRenderer.size;
        stationState = true;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            SwitchOn(stationState);
            stationState = !stationState;
        }
    }
    public void SwitchOn(bool value)
    {
        if (value)
        {
            isUploading = true;
            foreach (GameObject ob in detectedList)
            {
                ob.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
                ob.GetComponent<Collider2D>().enabled = false;
            }
            isUploading = false;
        }
        else
        {
            isUploading = true;
            PoolingReturn();
            foreach (GameObject ob in detectedList)
            {
                ob.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
                ob.GetComponent<Collider2D>().enabled = true;
            }
            isUploading = false;
        }
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
            activeList.Add(clone);
        }
        
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isUploading) return;
        if (IsInLayerMask(collision.gameObject, layerMask))
        {
            detectedList.Add(collision.gameObject);
        }

    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (isUploading) return;
        if (detectedList.Contains(collision.gameObject))
        {
            detectedList.Remove(collision.gameObject);
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
    private void PoolingReturn()
    {
        foreach(GameObject ob in activeList)
        {
            ob.SetActive(false);
        }
    }
}
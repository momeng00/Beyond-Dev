using System.Collections.Generic;
using UnityEngine;

public class UploadStation : Spot, ISwitchable, IReset
{
    public List<DownloadStation> partnerStations = new List<DownloadStation>();
    public List<Switch> switches;

    private BoxCollider2D col;
    private Animator ani;
    private Material material;
    private SpriteRenderer spriteRenderer;

    private bool stationState;

    private List<GameObject> detectedList = new List<GameObject>();

    [SerializeField]
    private List<GameObject> uploadList = new List<GameObject>();

    private List<GameObject> activeList = new List<GameObject>();
    private Dictionary<(GameObject original, DownloadStation station), GameObject> readyList = new Dictionary<(GameObject original, DownloadStation station), GameObject>();
    private DownloadStation selectedDownloadStation;

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

        // SpriteRenderer의 크기에 Collider 크기 맞추기
        col.size = spriteRenderer.size;

        // Sprite Pivot이 우측 상단이므로
        // Collider 중심을 좌측 아래로 Size의 절반만큼 이동
        col.offset = new Vector2(
            -spriteRenderer.size.x * 0.5f,
            -spriteRenderer.size.y * 0.5f
        );

        stationState = false;

        GameManager.Instance.OnReset += ResetAction;
    }

    public void invisibleBlock()
    {
        if (!stationState || !object.ReferenceEquals(selectedDownloadStation, null))
            return;

        foreach (GameObject sp in detectedList)
        {
            sp.GetComponent<SpriteRenderer>().enabled = false;
        }

        foreach (DownloadStation requester in partnerStations)
        {
            if (requester == null)
                continue;

            foreach (GameObject ob in uploadList)
            {
                Vector3 comparative = transform.position - ob.transform.position;

                GameObject clone = PoolingGet(ob, requester);

                clone.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
                clone.GetComponent<Collider2D>().enabled = false;

                if (!clone.activeSelf)
                {
                    clone.SetActive(true);
                }

                clone.GetComponent<SpriteRenderer>().enabled = true;

                clone.transform.position =
                    requester.transform.position - comparative;

                if (!requester.blocks.Contains(clone))
                {
                    requester.blocks.Add(clone);
                }

                if (clone.TryGetComponent(out PushBlock clonePushBlock))
                {
                    clonePushBlock.UDAnimationPlay(true);
                    Debug.Log(clonePushBlock);
                }

                if (ob.TryGetComponent(out PushBlock pushBlock))
                {
                    pushBlock.UDAnimationPlay(false);
                    Debug.Log(pushBlock);
                }

                if (!activeList.Contains(clone))
                {
                    activeList.Add(clone);
                }
            }
        }
    }

    public bool SwitchOn(bool value)
    {
        if (!value)
        {
            if (!stationState)
                return false;

            PoolingReturn();
            return true;
        }

        detectedList.RemoveAll(ob => ob == null);
        if (stationState || detectedList.Count == 0)
            return false;

        stationState = true;
        isUploading = true;
        uploadList.Clear();
        uploadList.AddRange(detectedList);

        foreach (GameObject ob in uploadList)
        {
            ob.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
            ob.GetComponent<Collider2D>().enabled = false;
        }

        foreach (DownloadStation ds in partnerStations)
        {
            if (ds != null)
                ds.blocks.Clear();
        }

        invisibleBlock();

        foreach (DownloadStation ds in partnerStations)
        {
            if (ds != null)
                ds.UploadComplete(true);
        }

        isUploading = false;
        return true;
    }

    public bool TrySelectDownload(DownloadStation requester)
    {
        if (!stationState || isUploading || requester == null
            || !object.ReferenceEquals(selectedDownloadStation, null)
            || !partnerStations.Contains(requester) || uploadList.Count == 0)
            return false;

        foreach (GameObject original in uploadList)
        {
            if (original == null
                || !readyList.TryGetValue((original, requester), out GameObject clone)
                || clone == null || !requester.blocks.Contains(clone))
                return false;
        }

        selectedDownloadStation = requester;
        foreach (DownloadStation ds in partnerStations)
        {
            if (ds != null)
                ds.UploadComplete(false);
        }

        return true;
    }

    public void partnerInit()
    {
        foreach (DownloadStation partner in partnerStations)
        {
            if (partner != null)
            {
                partner.GetPartnerDate(
                    transform.position,
                    spriteRenderer.size
                );
            }
        }
    }

    public void PoolingReturn()
    {
        isUploading = true;
        stationState = false;
        selectedDownloadStation = null;

        foreach (DownloadStation ds in partnerStations)
        {
            if (ds == null)
                continue;

            ds.UploadComplete(false);
            ds.blocks.Clear();
        }

        foreach (GameObject ob in uploadList)
        {
            if (ob == null)
                continue;

            ob.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
            ob.GetComponent<Collider2D>().enabled = true;
            ob.GetComponent<SpriteRenderer>().enabled = true;

            if (ob.TryGetComponent(out PushBlock pushBlock))
                pushBlock.UDAnimationPlay(true);
        }

        foreach (GameObject ob in activeList)
        {
            if (ob == null)
                continue;

            ob.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
            ob.GetComponent<Collider2D>().enabled = false;
            if (ob.TryGetComponent(out PushBlock pushBlock))
                pushBlock.UDAnimationPlay(false);
            else
                ob.SetActive(false);
        }

        activeList.Clear();
        uploadList.Clear();
        isUploading = false;
    }

    public void GetDetectedObject(GameObject requester)
    {
        if (requester != null
            && requester.TryGetComponent(out DownloadStation station)
            && partnerStations.Contains(station))
            station.SwitchOn(true);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (isUploading)
            return;

        GameObject target = collision.gameObject;

        if (IsInLayerMask(target, layerMask) &&
            IsFullyContained(collision.bounds))
        {
            if (!detectedList.Contains(target))
            {
                target.GetComponent<Block>().OnBlockAction();
                detectedList.Add(target);
            }
        }
        else
        {
            detectedList.Remove(target);
        }

        ani.SetBool("IsDetected", detectedList.Count > 0);
    }

    private bool IsFullyContained(Bounds blockBounds)
    {
        return
            col.bounds.Contains(blockBounds.min) &&
            col.bounds.Contains(blockBounds.max);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (isUploading)
        {
            return;
        }

        if (detectedList.Contains(collision.gameObject))
        {
            detectedList.Remove(collision.gameObject);
        }

        if (detectedList.Count <= 0)
        {
            ani.SetBool("IsDetected", false);
        }
    }

    private GameObject PoolingGet(GameObject original, DownloadStation requester)
    {
        var key = (original, requester);
        if (!readyList.TryGetValue(key, out GameObject clone) || clone == null)
        {
            clone = Instantiate(original);
            readyList[key] = clone;
        }

        clone.GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
        clone.GetComponent<Rigidbody2D>().angularVelocity = 0f;
        return clone;
    }

    public void InitializeReset()
    {
    }

    public void ResetAction()
    {
        PoolingReturn();

        foreach (GameObject clone in readyList.Values)
        {
            if (clone != null)
                Destroy(clone);
        }

        readyList.Clear();
        detectedList.Clear();
    }
}
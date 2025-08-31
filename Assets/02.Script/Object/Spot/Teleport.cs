using System.Collections.Generic;
using UnityEngine;

public class Teleport : Spot, IClearCondition
{
    //게임의 클리어에 해당하는 기능을 구현해야합니다.
    public Transform arrivePos;
    public int stage;
    [SerializeField] private List<GameObject> detects = new List<GameObject>();
    [SerializeField] private bool isSatisfied;
    private Collider2D col;
    public void Initialize()
    {
        GameManager.Instance.RegisterCondition(stage, this);
        GameManager.Instance.RegisterClearAction(stage, ClearAction);
    }

    private void Start()
    {
        col = GetComponent<Collider2D>();
        col.isTrigger = true;
        Initialize();
    }
    public override void Interact()
    {
        base.Interact();
        foreach (GameObject go in detects)
        {
            go.transform.position = arrivePos.position;
        }
    }

    #region 게임 클리어를 위한 기능구현

    public bool IsSatisfied()
    {
        return isSatisfied;
    }

    public void ClearAction()
    {
        Interact();
    }

    #endregion



    #region Trigger 구현

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (IsInLayerMask(collision.gameObject, layerMask))
        {
            detects.Add(collision.gameObject);
            isSatisfied = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (detects.Contains(collision.gameObject))
        {
            detects.Remove(collision.gameObject);
        }
        isSatisfied = false;
    }

    #endregion

}
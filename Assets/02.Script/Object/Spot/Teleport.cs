using System.Collections.Generic;
using UnityEngine;

public class Teleport : Spot, IClearCondition
{
    public Vector2 arrivePos;
    public int stage;
    [SerializeField]private List<GameObject> detects = new List<GameObject>();
    [SerializeField]private bool isSatisfied;
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
            go.transform.position = arrivePos;
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



    #region 그냥 편의성 유틸
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube((Vector3)arrivePos,new Vector3(0.5f,0.5f,0));
    }
    #endregion
}
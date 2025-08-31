using UnityEngine;

public class ClearToken : Token, IClearCondition
{
    public int stage;
    public LayerMask layerMask;
    public Transform arrivePos;
    public Transform go;
    private Collider2D col;
    public bool isSatisfied = false;
    

    private void Awake()
    {
        GameManager.Instance.RegisterCondition(stage, this);
        GameManager.Instance.RegisterClearAction(stage, ClearAction);
    }
    public bool IsSatisfied()
    {
        return isSatisfied;
    }

    public void ClearAction()
    {
        go.transform.position = arrivePos.position;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (IsInLayerMask(collision.gameObject, layerMask))
        {
            go = collision.gameObject.GetComponent<Transform>();
            gameObject.GetComponent<SpriteRenderer>().enabled = false;
            //아이템을 먹었을때
            isSatisfied = true;
            
        }
    }
    virtual protected bool IsInLayerMask(GameObject obj, LayerMask mask)
    {
        return ((1 << obj.layer) & mask) != 0;
    }


}
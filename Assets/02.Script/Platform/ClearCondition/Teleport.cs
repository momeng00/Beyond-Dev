using UnityEngine;

public class Teleport : MonoBehaviour, IDetect, IClearCondition
{
    public Vector2 arrivePos;
    public int stage;
    [SerializeField]private CharacterControl player;
    [SerializeField]private bool isSatisfied;

    private void Start()
    {
        Initialize();
    }
    public void Initialize()
    {
        GameManager.Instance.RegisterCondition(stage, this);
        GameManager.Instance.RegisterClearAction(stage, ClearAction);
    }
    public void ClearAction()
    {
        player.transform.position = arrivePos;
    }
    public void DetectAction(GameObject sender)
    {
        if (sender.TryGetComponent<CharacterControl>(out _))
        {
            player = sender.GetComponent<CharacterControl>();
        }
    }

    public void DetectEnter()
    {
        isSatisfied = true;
    }

    public void DetectExit()
    {
        isSatisfied = false;
    }

    public bool IsSatisfied()
    {
        return isSatisfied;
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube((Vector3)arrivePos,new Vector3(0.5f,0.5f,0));
    }
}
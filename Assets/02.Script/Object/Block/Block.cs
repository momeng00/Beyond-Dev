using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static MatarialAnim;

public abstract class Block : MonoBehaviour, IReset
{
    //원본 데이터
    public List<PropertyData> propertyList;
    public float matarialAnimDuration = 0.25f;
    protected MatarialAnim matarialAnim = new MatarialAnim();
    protected Collider2D col;
    protected Rigidbody2D rb;
    public Action<bool> blockEvent;
    protected SpriteMask mask;
    [SerializeField]protected List<GameObject> PopUpList;
    private List<IEventListener> eventListeners = new List<IEventListener>();
    public float toggleDelay;
    private Coroutine activateCoroutine;
    [HideInInspector]public SpriteRenderer spriteRenderer;
    virtual public bool BlockState { get; set; }

    
    public virtual void Start()
    {
        if (gameObject.TryGetComponent(out SpriteRenderer sr))
        {
            matarialAnim.InitMatarialAnim(this,sr.material, propertyList, matarialAnimDuration);
        }
        mask = GetComponent<SpriteMask>();
        col = GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();
        GameManager.Instance.RegisterInitAction(ResetAction);
        GameManager.Instance.OnReset += ResetAction;
    }

    public virtual void OnBlockAction()
    {

    }
    public virtual void InitializeReset()
    {

    }

    public virtual void ResetAction()
    {

    }
    private void OnValidate()
    {
        for (int i = 0; i < PopUpList.Count; i++)
        {
            if (PopUpList[i] == null) continue;

            // IMovable이 없으면?
            if (PopUpList[i].GetComponent<IEventListener>() == null)
            {
                Debug.LogError($"{PopUpList[i].name}은(는) IEventListener 없습니다! 리스트에서 제거합니다.");
                PopUpList[i] = null; // 강제로 빼버림
            }
        }
    }
    protected void ToggleEventChildren()
    {
        //IEventListener[] listeners = GetComponentsInChildren<IEventListener>();
        //eventListeners = new List<IEventListener>(listeners);
        foreach (var obj in PopUpList)
        {
            // GameObject에서 인터페이스 추출 (TryGetComponent가 효율적)
            if (obj.TryGetComponent(out IEventListener evt))
            {
                eventListeners.Add(evt);
            }
            else
            {
                Debug.LogError($"{obj.name}에는 IEventListener이 없습니다!");
            }
        }
        eventListeners.Sort((a, b) => a.ToggleEventPriority.CompareTo(b.ToggleEventPriority));
    }
    public void RunToggleEvent(bool state)
    {
        if (state)
        {
            activateCoroutine = StartCoroutine(RunToggleEventDelay(state));
        }
        else
        {
            foreach (var listener in eventListeners)
            {
                listener.ToggleEvent(BlockState);
            }
            if (activateCoroutine == null)
                return;
            StopCoroutine(activateCoroutine);
        }
    }

    IEnumerator RunToggleEventDelay(bool state)
    {
        foreach (var listener in eventListeners)
        {
            listener.ToggleEvent(BlockState);
            yield return new WaitForSeconds(toggleDelay);
        }
    }
}

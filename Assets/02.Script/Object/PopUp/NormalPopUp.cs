using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;

public class NormalPopUp : PopUp, IEventListener, IReset
{
    public SmartKey key; //enum을 통해서 전달 받을꺼임.
    public SpriteRenderer image;
    public TextMeshPro nickName;
    public TextMeshPro content;
    public TextMeshPro profileID;
    public bool EventOnce = false;
    private bool eventFlag= false;
    private Animator animator;
    //[SerializeField]private Block Block;
    public bool openAnimation = false;
    public int toggleEventPriority;
    public int ToggleEventPriority =>  toggleEventPriority;
    protected List<Animator> childAnimators;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        FindChildAnimator();
        ToggleEvent(false);
        //InitPopUpDatas(); 지금은 데이터 테이블이 없어서 주석처리 해둠. 오류문뜨는거 싫어서
    }
    private void Start()
    {
        GameManager.Instance.OnReset += ResetAction;
    }

    public virtual void FindChildAnimator()
    {
        childAnimators = new List<Animator>();
        Animator[] allAnimators = GetComponentsInChildren<Animator>(true);
        foreach (Animator anim in allAnimators)
        {
            if (anim.gameObject != this.gameObject)
            {
                childAnimators.Add(anim);
            }
        }
    }
    
    public virtual void ToggleEvent(bool state, Transform origin = null)
    {

        if (EventOnce)
            return;
        if(animator != null)
            animator.Play(state ? "In" : "Out");
        foreach (Animator anim in childAnimators)
        {
            anim.SetBool("IsActive", state);
        }
        
    }

    public void InitPopUpDatas()
    {
        PopUpData data = PopUpDataManager.Instance.GetData(key.key.ToString());
        nickName.text = data.Name.ToString();
        content.text = data.Content.ToString();
        profileID.text = data.profileID.ToString();
        Sprite[] allSprites = Resources.LoadAll<Sprite>("RandomProfile");
        Sprite targetSprite = allSprites.FirstOrDefault(s => s.name == data.profileImage);
        if (targetSprite != null)
            image.sprite = targetSprite;
    }

    public void InitializeReset()
    {
        throw new System.NotImplementedException();
    }

    public void ResetAction()
    {
        eventFlag = false;
        ToggleEvent(false);
        eventFlag = false;
    }
}

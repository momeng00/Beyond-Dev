using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;

public class NormalPopUp : PopUp, IEventListener
{
    public SmartKey key; //enum을 통해서 전달 받을꺼임.
    public SpriteRenderer image;
    public TextMeshPro nickName;
    public TextMeshPro content;
    public TextMeshPro profileID;
    public bool EventOnce = false;
    private Animator animator;
    //[SerializeField]private Block Block;

    public int toggleEventPriority;
    public int ToggleEventPriority =>  toggleEventPriority;
    protected List<Animator> childAnimators;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        FindChildAnimator();
        //InitPopUpDatas(); 지금은 데이터 테이블이 없어서 주석처리 해둠. 오류문뜨는거 싫어서
    }


    public void FindChildAnimator()
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

    public void ToggleEvent(bool state)
    {
        if (EventOnce)
            return;
        animator.Play(state ? "In" : "Out");
        foreach (Animator anim in childAnimators)
        {
            anim.SetBool("IsActive", state);
        }
        
    }

    private void InitPopUpDatas()
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
}

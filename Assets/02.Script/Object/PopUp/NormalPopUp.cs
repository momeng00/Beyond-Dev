using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NormalPopUp : PopUp, IEventListener
{
    //public key 나중에 키 생기면 추가하셈. enum을 통해서 전달 받을꺼임.
    public SpriteRenderer image;
    public TextMeshPro nickName;
    public TextMeshPro content;
    public TextMeshPro tagID;

    private Animator animator;
    //[SerializeField]private Block Block;

    public int toggleEventPriority;
    public int ToggleEventPriority =>  toggleEventPriority;
    protected List<Animator> childAnimators;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        FindChildAnimator();
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
        animator.Play(state ? "In" : "Out");
        foreach (Animator anim in childAnimators)
        {
            anim.SetBool("IsActive", state);
        }
        
    }
}
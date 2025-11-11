using System.Collections.Generic;
using UnityEngine;

public class NormalPopUp : PopUp, IEventListener
{
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

    public override void PopUpHandle(bool state)
    {
        base.PopUpHandle(state);
        gameObject.SetActive(state);
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
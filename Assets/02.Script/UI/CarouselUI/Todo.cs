using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Todo : MonoBehaviour
{
    [SerializeField]private bool isClear=false;
    private InGameTodo gameTodo;
    private Animator ani;
    public TMP_Text content_text;
    public Action OnAnimationFinished; // 애니메이션이 끝날때 카드에게 반환되기 위한 메서드

    public void SubscribeTodoAction(Action action)
    {
        OnAnimationFinished += action;
    }

    private void Awake()
    {
        gameTodo = FindFirstObjectByType<InGameTodo>();
        //content_text = GetComponent<TMP_Text>(); //인스펙터에서 직접 할당으로 변경
        ani = GetComponent<Animator>();
    }
    public void AnimationTodoDone()
    {
        //Todo 애니메이션인 Todo_Done이 호출하는 이벤트
        OnAnimationFinished?.Invoke();
    }
    public void CheckClear()
    {
        if (isClear)
        {
            ani.Play("Todo_Done");
        }
        else
        {
            ani.Play("Todo_Default");
            //여기서 강제로 AnimationTodoDone을 실행시켜야할듯?
            //얘네는 
        }
    }
    public void Close()
    {
        ani.Play("Todo_Default");
    }
    public void DoSuccess()
    {
        isClear = true;
        gameTodo.BannerNotice(content_text.text);
    }
}
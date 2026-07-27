using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Todo : MonoBehaviour
{
    [SerializeField]private bool isClear=false;
    private InGameTodo gameTodo;
    private Animator ani;
    public TMP_Text content_text; 
    private void Awake()
    {
        gameTodo = FindFirstObjectByType<InGameTodo>();
        //content_text = GetComponent<TMP_Text>(); //인스펙터에서 직접 할당으로 변경
        ani = GetComponent<Animator>();
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
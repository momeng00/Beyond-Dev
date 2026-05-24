using Unity.VisualScripting;
using UnityEngine;

public class Todo : MonoBehaviour
{
    [SerializeField] private bool isClear=false;
    private Animator ani;
    private void Awake()
    {
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
    }
}
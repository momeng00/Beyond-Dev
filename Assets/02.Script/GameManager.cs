using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.AI;
using static System.Runtime.CompilerServices.RuntimeHelpers;

public class GameManager : MonoBehaviour
{
    private int stage = 1;
    private Dictionary<int,List<IClearCondition>> condition = new Dictionary<int, List<IClearCondition>>();
    private Dictionary<int, Action> clearAction = new Dictionary<int, Action>();
    private Action initAction;
    public Action OnReset;
    private static GameManager _instance;
    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new GameObject("GameSystem").AddComponent<GameManager>();
            }
            return _instance;
        }
    }

    public void CheckClear()
    {
        if (condition[stage]==null)
        {
            return;
        }
        bool clear = true;
        foreach(var condition in condition[stage])
        {
            if (!condition.IsSatisfied())
            {
                clear = false; 
                break;
            }
        }
        if (clear)
        {
            clearAction[stage]?.Invoke();
            //임시용
            NextStage();
            initAction?.Invoke();
        }
    }
    public void NextStage() //임시용
    {
        if (condition.ContainsKey(stage+1))
        {
            stage++;
        }
        else
        {
            Debug.Log("stage ERROR 끝이 났거나 오류가 발생!");
            stage = 0;
        }
    }
    public void RegisterCondition(int stage, IClearCondition condition)
    {
        if (!this.condition.ContainsKey(stage))
        {
            this.condition[stage] = new List<IClearCondition>();
        }
        this.condition[stage].Add(condition);
    }

    public void RegisterClearAction(int stage, Action act)
    {
        if (!clearAction.ContainsKey(stage))
        {
            clearAction[stage] = act;
            return;
        }
        clearAction[stage] += act;
    }
    public void RegisterInitAction(Action act)
    {
        if (!clearAction.ContainsKey(stage))
        {
            clearAction[stage] = act;
            return;
        }
        clearAction[stage] += act;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        RegisterCondition(0,new DummyCondition());
    }

    // Update is called once per frame
    void Update()
    {
        CheckClear();
        if (Input.GetKeyDown(KeyCode.R))
        {
            OnReset?.Invoke();
        }
    }
}
public class DummyCondition : IClearCondition
{
    public void ClearAction()
    {
        
    }

    public bool IsSatisfied() => false; // 무조건 클리어 조건 통과
}
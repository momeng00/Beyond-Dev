using System;
using System.Collections.Generic;
using UnityEngine;
using static System.Runtime.CompilerServices.RuntimeHelpers;

public class GameManager : MonoBehaviour
{
    private int stage = 1;
    private Dictionary<int,List<IClearCondition>> condition = new Dictionary<int, List<IClearCondition>>();
    private Dictionary<int, Action> clearAction = new Dictionary<int, Action>();
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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        CheckClear();
    }
}

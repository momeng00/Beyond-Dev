using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Stage : MonoBehaviour
{
    public string stageName;
    public Stage nextStage;
    public List<ClearCondition> conditionItems;
    public UnityEvent EnterEvent;
    public UnityEvent ExitEvent;

    private void Start()
    {
        foreach (var condition in conditionItems)
        {
            condition.AddOnCheck(StageSatisfied);
        }
    }

    private void NextStage()
    {
        if (nextStage != null)
        {
            GameManager.Instance.NextStage(nextStage);
        }
        else
        {
            ExitEvent?.Invoke();
        }
    }

    public void StageEnter()
    {
        EnterEvent?.Invoke();
        GameManager.Instance.initAction?.Invoke();
    }

    public void StageExit()
    {
        ExitEvent?.Invoke();
    }

    private void StageSatisfied()
    {
        foreach (var condition in conditionItems)
        {
            if (!condition.IsSatisfied())
            {
                return;
            }
        }
        NextStage();
    }
}
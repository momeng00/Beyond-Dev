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
    private bool _canCheckConditions;
    private bool _isCompleted;

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
        _canCheckConditions = false;
        _isCompleted = false;

        EnterEvent?.Invoke();
        GameManager.Instance.initAction?.Invoke();

        _canCheckConditions = true;
    }

    public void StageExit()
    {
        _canCheckConditions = false;

        ExitEvent?.Invoke();
    }

    private void StageSatisfied()
    {
        if (!_canCheckConditions || _isCompleted)
            return;

        var manager = GameManager.Instance;

        if (manager == null || manager.currentStage != this)
            return;

        // 잘못 연결된 조건을 클리어로 취급하지 않는다.
        if (conditionItems == null || conditionItems.Count == 0)
            return;

        foreach (var condition in conditionItems)
        {
            if (condition == null || !condition.IsSatisfied())
            {
                return;
            }
        }

        _isCompleted = true;
        _canCheckConditions = false;

        NextStage();
    }
}
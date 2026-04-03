using System;

public interface IClearCondition
{
    bool IsSatisfied();
    void ClearAction();
}
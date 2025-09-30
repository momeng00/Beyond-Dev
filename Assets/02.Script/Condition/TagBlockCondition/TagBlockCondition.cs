using UnityEngine;

public abstract class TagBlockCondition : ScriptableObject
{
    public abstract bool IsSatisfied(TagBlockController controller, string groupName);
}

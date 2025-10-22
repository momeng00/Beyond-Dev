using System.Collections.Generic;
using UnityEngine;

public interface IMovable
{
    public Dictionary<object, Vector2> ExtraVelocityList { get; }
    public void AddExtraVelocity(object root, Vector2 force);
    public void RemoveExtraVelocity(object root);
    public Vector2 CalculateVelocity();
}
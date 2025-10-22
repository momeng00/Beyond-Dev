using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SimplePhysics : MonoBehaviour, IMovable
{
    private Rigidbody2D _rb;
    private Dictionary<object,Vector2> extraVelocityList = new Dictionary<object,Vector2>();
    public Dictionary<object, Vector2> ExtraVelocityList => extraVelocityList;
    private Vector2 _extraVelocity;
    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }
    private void FixedUpdate()
    {
        _rb.linearVelocity = new Vector2 (_extraVelocity.x,_rb.linearVelocityY);
    }
    public void AddExtraVelocity(object root, Vector2 force)
    {
        extraVelocityList[root] = force;
        _extraVelocity = CalculateVelocity();
    }
    public void RemoveExtraVelocity(object root)
    {
        if (extraVelocityList.ContainsKey(root))
        {
            extraVelocityList.Remove(root);
        }
        _extraVelocity = CalculateVelocity();
    }
    public Vector2 CalculateVelocity()
    {
        return extraVelocityList.Values.Aggregate(Vector2.zero, (acc, v) => acc + v);
    }
}
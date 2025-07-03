using UnityEngine;

[CreateAssetMenu(fileName = "Stat", menuName = "Scriptable Objects/Stat")]
public class Stat : ScriptableObject
{
    public float moveSpeed;
    public float jumpForce;
}
public enum CharacterStat
{
    NormalStat,
    SlowStat,
    FastStat,

}
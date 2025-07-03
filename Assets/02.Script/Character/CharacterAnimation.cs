using UnityEngine;

public class CharacterAnimation : MonoBehaviour
{
    //ID를 전달하면 클래스를 반환하게 DIctionary를 통해서 구현을 했었음, 단 DataSheet를 만들어서 미리 선언을 해둬야함.
    public CharacterControl characterControl;
    public CharacterStateID previousCharacterStateID;
    public CharacterStateID currentCharacterState;
    
    public void Init()
    {

    }
    private void Update()
    {
        
    }
    public void UpdateState()
    {

    }

    public void ChangeState(CharacterStateID nextStateID)
    {

    }
}
public enum CharacterStateID
{
    None,
    Move,
    Jump,
    Falling,
    Fallen,
    Die,
    Slow,
    Push,
}
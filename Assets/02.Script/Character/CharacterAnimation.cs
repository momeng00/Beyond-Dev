using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CharacterAnimation : MonoBehaviour
{
    //ID를 전달하면 클래스를 반환하게 DIctionary를 통해서 구현을 했었음, 단 DataSheet를 만들어서 미리 선언을 해둬야함.
    private Dictionary<CharacterStateID,CharacterStateBase> stateData = new Dictionary<CharacterStateID, CharacterStateBase>();
    public CharacterControl characterControl;
    public Collider2D col;
    public Rigidbody2D rb;
    public CharacterStateID previousCharacterStateID;
    public CharacterStateID currentCharacterStateID;
    
    public void Init()
    {
        characterControl = GetComponent<CharacterControl>();
        stateData = CharacterStateSheet.GetStateDate(this);
        col = characterControl.GetComponent<Collider2D>();
        rb = characterControl.GetComponent<Rigidbody2D>();
    }
    private void Start()
    {
        Init();
    }
    private void Update()
    {
        UpdateState();
    }
    public void UpdateState()
    {
        CharacterStateID next = stateData[currentCharacterStateID].OnUpdateState();
        ChangeState(next);
    }

    public bool ChangeState(CharacterStateID nextStateID)
    {
        if(nextStateID == currentCharacterStateID)
            { return false; }
        if(!stateData[nextStateID].canExecute)
            { return false; }

        stateData[currentCharacterStateID].ExitState();
        previousCharacterStateID = currentCharacterStateID;
        currentCharacterStateID = nextStateID;
        stateData[currentCharacterStateID].EnterState();

        return true;
    }
}
public enum CharacterStateID
{
    Idle,
    Move,
    Jump,
    Falling,
    Landing,
    Die,
    Slow,
    Push,
}
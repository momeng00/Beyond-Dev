using UnityEngine;

public class CharacterAbility : MonoBehaviour
{
    private CharacterControl CharacterControl;


    private void Start()
    {
        CharacterControl = GetComponent<CharacterControl>();  
    }

    private void EnterAbility()
    {

    }
    private void UpdateAbility()
    {

    }
    
}

public enum CharacterAbilitySort
{
    Clearg
}
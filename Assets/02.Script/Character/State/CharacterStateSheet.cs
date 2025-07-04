
using System.Collections.Generic;

public static class CharacterStateSheet
{

    public static Dictionary<CharacterStateID, CharacterStateBase> GetStateDate(CharacterAnimation machine)
    {
        return new Dictionary<CharacterStateID, CharacterStateBase>()
        {
            { CharacterStateID.Move, new Move(machine) },
            { CharacterStateID.Jump, new Jump() },
            { CharacterStateID.Falling, new Falling() },
            { CharacterStateID.Landing,new Landing() },
            { CharacterStateID.Slow, new Slow() },
            { CharacterStateID.Idle, new Idle(machine) },
            { CharacterStateID.Push, new Push() },
            { CharacterStateID.Die, new Die() },

        };
    }
}
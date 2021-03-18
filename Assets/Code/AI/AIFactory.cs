using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AIFactory
{
    public static AIBoard Init_AIBoard(AIBoard template)
    {
        AIBoard board = ScriptableObject.Instantiate(template);

        return board;
    }
}
